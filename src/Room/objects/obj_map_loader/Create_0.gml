/// @description Initialize default parameters.

instance_singleton(obj_map_loader);

/// @type {String}
/// @description The TMX map data.
map_data = undefined;

/// @type {Struct.Logger}
/// @description The logger.
_logger = new Logger(nameof(obj_map_loader));

/// @type {Struct.MapClient}
/// @description The map client used to fetch and load the servers map.
_map_client = new MapClient({ x_api_key: api_key });

_logger.log(log_type.information, "Loading map...");

_map_client
	.get_async("bellmare_tavern")
	.next(method(self, function(response)
	{
		map_data = response.data;
		map_load_map(response.data);

		layer_set_visible("Collisions", debug_mode || os_type != os_linux);
		
		_logger.log(log_type.information, $"Loaded map: '{response.name}'");
	}))
	.fail(method({_logger}, function(error)
	{
		_logger.log(log_type.error, $"Failed to load map data: {error}");
		throw new Error($"Failed to map load map data", error);
	}));