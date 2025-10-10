import http from 'http';
import { startWssServer } from './wss/wssServer.js';

const server = http.createServer();
const port = 8080;

startWssServer(server);

server.listen(port, () => {
    console.info(`Server listening on port: ${port}`);
});