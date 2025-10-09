/// @description Represents a connection to server.
/// @param {Id.Socket} socket The underlying socket connection.
function ClientConnection(socket) constructor
{
	/// @type {Id.Socket}
	/// @description The underlying socket connection.
	_socket = socket;

	/// @type {Id.Buffer}
	/// @description The buffer used to send notifications, errors and results to the client.
	_write_buffer = buffer_create(1, buffer_grow, 1);
	
	/// @description Sends a JSON-RPC 2.0 notification to the client.
	/// @param {String} method The name of the method to invoke on the client.
	/// @param {Any} params The parameters of the method/procedure to send to the client.
    notify = function(method, params = undefined)
    {
		var payload = {
            jsonrpc: "2.0",
            method: method,
            params: params,
        };
		
		buffer_seek(_write_buffer, buffer_seek_start, 0);
        buffer_write(_write_buffer, buffer_string, json_stringify(payload));
		network_send_packet(_socket, _write_buffer, buffer_tell(_write_buffer));
    }
	
	/// @description Sends a JSON-RPC 2.0 response with a result (success).
	/// @param {Real} rpc_id The remote procedure call identifier.
	/// @param {Any} result The result to send to the client.
	send_result = function(rpc_id, result)
	{
		var payload = {
            jsonrpc: "2.0",
            id: rpc_id,
            result: result
        };
		
		buffer_seek(_write_buffer, buffer_seek_start, 0);
        buffer_write(_write_buffer, buffer_string, json_stringify(payload));
		network_send_packet(_socket, _write_buffer, buffer_tell(_write_buffer));
	}
	
	/// @description Sends a JSON-RPC 2.0 response with an error (failure).
	/// @param {Real|Undefined} rpc_id The remote procedure call identifier.
	/// @param {Real} code The error code.
	/// @param {String} message The error message.
	/// @param {Struct} data The error data (if any).
	send_error = function(rpc_id, code, message, data = {})
	{
		var payload = {
			jsonrpc: "2.0",
			id: rpc_id,
			error: {
				code: code,
				message: message,
				data: data,
			},
		};
		
		buffer_seek(_write_buffer, buffer_seek_start, 0);
        buffer_write(_write_buffer, buffer_string, json_stringify(payload));
		network_send_packet(_socket, _write_buffer, buffer_tell(_write_buffer));
	}
	
	/// @description Disconnects the client from the server.
	disconnect = function()
	{
		buffer_delete(_write_buffer);
		network_destroy(_socket);
	}
}