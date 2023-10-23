let apiEndpoint = "https://localhost:7139";
let boxWid = 20;         // Width of each cell
let resizedBoxWid = 20;  // Width of each cell when zoomed in/out
let tableArr = [];       // 2D array of grid. 1 - Alive, 0 - Dead
let fps = 30;            // 30 fps at start, 1 fps when the game is active
let gridLn = 30;         // Grid is size gridLn x gridLn
let state = 0;           // Holds the state if the game is active or not
let canvas;              // Canvas to render the grid
let intervalId;          // Id to stop the requests to the server

// Zoom factor
let zoom = 1;
let lastTouchDistance = 0;

// Event listener for pinch-to-zoom on touch devices
let initialTouchDistance = 0;
let initialZoom = 1;

window.onload=function(){
    
    // Get references to HTML elements
    const startButton = document.getElementById("startButton");
    const stopButton = document.getElementById("stopButton");
    const resetButton = document.getElementById("resetButton");

    // Add event listeners for buttons
    startButton.addEventListener("click", startGame);
    stopButton.addEventListener("click", stopGame);
    resetButton.addEventListener("click", resetGame);    
}

function setup() {
    frameRate(fps);
    canvas = createCanvas(600, 600); // Set the initial canvas size
    canvas.parent("sketch-container"); // Place the canvas inside a container
    initializeTable();
    
    canvas.touchStarted(function (event) {
        if (touches.length === 2) {
            initialTouchDistance = dist(touches[0].x, touches[0].y, touches[1].x, touches[1].y);
            initialZoom = zoom;
        }
    });

    canvas.touchMoved(function (event) {
        if (touches.length === 2) {
            let currentTouchDistance = dist(touches[0].x, touches[0].y, touches[1].x, touches[1].y);
            zoom = constrain(initialZoom * (currentTouchDistance / initialTouchDistance), 1, 3);
        }
    });
}


function startGame() {
    // Disable the Start button and enable the Stop button
    startButton.disabled = true;
    stopButton.disabled = false;

    changeErrorMessage("");
    if (state === 0) {
        // If the game hasn't yet started
        fps = 1;
        frameRate(fps);
        state = 1;
    }            
    startRequests();
}

function stopGame() {    
    clearInterval(intervalId);      
    state = 0;

    // Enable the Start button and disable the Stop button
    startButton.disabled = false;
    stopButton.disabled = true;
}

function resetGame() {
    tableArr = [];
    initializeTable();    
    redraw();
    stopGame();     
}

//Initialize table array with 0s
function initializeTable()
{
    for (let r = 0; r < gridLn; r++) {
        let rowArr = [];
        for (let c = 0; c < gridLn; c++) {
            rowArr.push(0);
        }
        tableArr.push(rowArr);
    }
}

//Starts the per-second request for the next generation of the game state
async function startRequests()
{
    try
    {
        intervalId = setInterval(async () => {
            result = await sendRequest();
            if (result === undefined)
            {                        
                console.error("Error processing request from server.");
                resetGame();
                return;
            }
            tableArr = result;
        }, 1000); 
    }
    catch (err) {
        changeErrorMessage("Error processing request from server.");
        resetGame();
    }
}


//Control zoom via mouse scroll
function mouseWheel(event) {
    if (state === 1) {
        return;
    }
    zoom += event.delta * -0.01;
    zoom = constrain(zoom, 1, 3); // Limit the zoom factor
    resizedBoxWid = boxWid*zoom;
}

//Tags each cell for the setup
function mousePressed() {
    let row = (mouseX - (mouseX % resizedBoxWid)) / resizedBoxWid; // Grab the nearest row above click
    let col = (mouseY - (mouseY % resizedBoxWid)) / resizedBoxWid; // Grab the nearest col left of click
    
    if (row <= gridLn && col <= gridLn) {
        // Valid row, col
        tableArr[row][col] = -1 * tableArr[row][col] + 1; // Invert the cell
        redraw(); // Redraw table
    }
}

//Renders the grid
function draw() {
    scale(zoom); // Apply zoom
    translate(0, 0);
    background(255); // Clear the canvas
    tableArr.forEach((rowArr, row) => {
        rowArr.forEach((colVal, col) => {
        fill(colVal === 1 ? "black" : "transparent"); // Black if live, transparent if dead
        rect(row * boxWid, col * boxWid, boxWid, boxWid);
        });
    });
}


function changeErrorMessage(errorMessage)
{
    document.getElementById("error-message").textContent=errorMessage;
}


//Sends the request to the server to get next state
function sendRequest() {
    // Encode the parameter to make it safe for URL inclusion
    let param = arrayToString(tableArr);
    const encodedParam = encodeURIComponent(param);

    // Construct the URL with the parameter
    const apiUrl = `${apiEndpoint}/api/Game/getnextgen/${encodedParam}`;
    // Send the GET request
    return fetch(apiUrl)
        .then((response) => {
            // Check if the response status is OK (200)
            if (!response.ok) {
                throw new Error(`Request failed with status: ${response.status}`);
            }

            // Convert the response to JSON
            return response.json();
        })
        .then((data) => {
            // Assuming the data is in a format like: [["value1", "value2"], ["value3", "value4"], ...]
            return data;
        })
        .catch((error) => {
            console.error("Error:", error);
            changeErrorMessage("Error connecting to server.");
            resetGame();
        }
    );
}


function arrayToString(arr) {
    if (!Array.isArray(arr)) {
        return arr.toString();
    }

    arr = arr.filter(function (value) {
        return !Number.isNaN(value);
    });

    const innerStrings = arr.map(subArray => arrayToString(subArray));
    return `[${innerStrings.join(',')}]`;
}
