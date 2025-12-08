/// @description Draw the debug grid.

#region Debug View

if (_draw_grid)
{
	draw_set_alpha(_alpha);
	mp_grid_draw(_grid);
	draw_set_alpha(1.0);
}

#endregion
