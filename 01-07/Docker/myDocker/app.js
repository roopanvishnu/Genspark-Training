// server.js
const fs = require('fs');
const path = require('path');

const logFile = path.join('/app/data', 'log.txt');

fs.appendFileSync(logFile, 'Container started\n');

console.log("App is running. Press Ctrl+C to stop.");

// Keep the container running
setInterval(() => {
  fs.appendFileSync(logFile, 'Heartbeat...\n');
}, 5000);
