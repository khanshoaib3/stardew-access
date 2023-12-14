# Read Tile Commands

commands-read_tile-read_tile_toggle = Read Tile is {$is_enabled ->
    [0] off.
    *[1] on.
  }
commands-read_tile-watered_toggle = Speaking {$is_enabled ->
    [0] un-watered
    *[1] watered
  } for crops.
commands-read_tile-flooring_toggle = Speaking floorings is {$is_enabled ->
    [0] off.
    *[1] on.
  }

# Tile Marking Commands

commands-tile_marking-build_list-building_info = Index {$index}: {$name} at {$x_position}x and {$y_position}y
commands-tile_marking-build_list-buildings_list = Available buildings:
  {$building_infos}
  Open command menu and use pageup and pagedown to check the list
commands-tile_marking-build_list-no_building = No appropriate buildings to list

commands-tile_marking-mark-location_marked = Location {$x_position}x {$y_position}y added at {$index} index.
commands-tile_marking-mark-not_in_farm = Can only use this command in the farm
commands-tile_marking-mark-index_not_entered = Enter the index too!
commands-tile_marking-mark-wrong_index = Index can only be a number and from 0 to 9 only

commands-tile_marking-mark_list-mark_info = Index {$index}: {$x_position}x and {$y_position}y
commands-tile_marking-mark_list-marks_list = Marked positions:
  {$mark_infos}
  Open command menu and use pageup and pagedown to check the list
commands-tile_marking-mark_list-not_marked = No positions marked!

commands-tile_marking-build_sel-cannot_select = Cannot select building.
commands-tile_marking-build_sel-building_index_not_entered = Enter the index of the building too! Use buildlist.
commands-tile_marking-build_sel-marked_index_not_entered = Enter the index of marked place too! Use marklist.
commands-tile_marking-build_sel-wrong_index = Index can only be a number.
commands-tile_marking-build_sel-no_building_found = No building found with index {$index}. Use buildlist.
commands-tile_marking-build_sel-no_marked_position_found = No marked position found at {$index} index.

# Other Commands

commands-other-tts_toggle = TTS is {$is_enabled ->
    [0] off.
    *[1] on.
  }
commands-other-warnings_toggle = Warnings is {$is_enabled ->
    [0] off.
    *[1] on.
  }
commands-other-snap_mouse_toggle = Snap Mouse is {$is_enabled ->
    [0] off.
    *[1] on.
  }
commands-other-hns_percentage_toggle = Speaking health and stamina in {$is_enabled ->
    [0] normal
    *[1] percentage
  } format
commands-other-refresh_mod_config = Mod configs refreshed!
commands-other-refresh_screen_reader = Screen reader refreshed!
