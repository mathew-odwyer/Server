/// @description Initialize services.

/// @globalvar {Struct.Logger} Logger
/// @globalvar {Struct.EventAggregator} EventAggregator 
/// @globalvar {Any} Promise

Logger.LogLevel = log_type.debug;

exception_unhandled_handler(unhandled_exception_callback);

date_set_timezone(timezone_utc);

instance_create_layer(0, 0, "Instances", obj_http);
instance_create_layer(0, 0, "Instances", obj_server);
instance_create_layer(0, 0, "Instances", obj_map_loader);

draw_enable_drawevent(os_type != os_linux);
