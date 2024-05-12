# Features

## Object Tracker

feature-object_tracker-sort_by_proximity = Sort by Proximity: {$is_enabled ->
    [0] Disabled
    *[1] Enabled
  }
feature-object_tracker-read_selected_object = {$object_name} {$only_tile ->
    [0] is {$direction} {$distance}
    *[1] {EMPTYSTRING()}
  } at {$object_x}-{$object_y}, player at {$player_x}-{$player_y}
feature-object_tracker-read_selected_coordinates = Coordinates {$coordinates} {$only_tile ->
    [0] is {$direction} {$distance}
    *[1] {EMPTYSTRING()}
  } player at {$player_x}-{$player_y}

feature-object_tracker-moving_to = Moving to {$object_x}-{$object_y}.
feature-object_tracker-could_not_find_path = Could not find path to object.
feature-object_tracker-no_objects_found = No objects found.
feature-object_tracker-no_categories_found = No categories found.
feature-object_tracker-end_of_list = End of list.
feature-object_tracker-start_of_list = Start of list.
feature-object_tracker-no_object = No Object
feature-object_tracker-no_category = No Category
feature-object_tracker-no_selected_object = No selected object
feature-object_tracker-read_favorite = Favorite {$favorite_number} is {$target_object} ({$target_category})
feature-object_tracker-read_favorite_stack  = Favorite stack {$stack_number}
feature-object_tracker-save_coordinates_toggle = Save coordinates: {$is_enabled ->
    [0] Disabled
    *[1] Enabled
  }
feature-object_tracker-favorite_unset = Favorite {$favorite_number} is unset
feature-object_tracker-favorite_save = Saving {$selected_object} ({$selected_category}) to {$location_name} favorite {$favorite_number}
feature-object_tracker-favorite_save_coordinates = Saving coordinates {$coordinates} to {$location_name} favorite {$favorite_number}
feature-object_tracker-favorite_cleared = {$location_name} favorite {$favorite_number} cleared
feature-object_tracker-favorite_set_as_default = Saving current favorites as default
feature-object_tracker-favorite_default_cleared = Default favorites cleared
feature-object_tracker-no_destination_selected = No destination selected

## Tile Viewer

feature-tile_viewer-relative_cursor_lock_info = Relative cursor lock: {$is_enabled ->
    [0] Disabled
    *[1] Enabled
  }
feature-tile_viewer-moving_to = Moving to {$tile_x}-{$tile_y}.
feature-tile_viewer-cannot_move_to = Cannot move to {$tile_x}-{$tile_y}.
feature-tile_viewer-stopped_moving = Stopped moving
feature-tile_viewer-reached = Reached destination
feature-tile_viewer-blocked_tile_name = blocked
feature-tile_viewer-empty_tile_name = empty

## Read Tile

feature-read_tile-manually_triggered_info = {$tile_name}, Category: {$tile_category}
feature-read_tile-no_tile_found = No tiles found at the given coordinates.
feature-read_tile-tile_indexes = Tile indexes:

## Other

feature-speak_selected_slot_item_name = {$slot_item_name} Selected
feature-speak_location_name = {$location_name} Entered
feature-speak_health_n_stamina-in_percentage_format = Health is {$health} % and Stamina is {$stamina} %
feature-speak_health_n_stamina-in_normal_format = Health is {$health} and Stamina is {$stamina}
feature-speak_money = You have {$money}g
feature-speak_time_and_season = Time is {$time_of_day} and it is {$day} {$date} of {$season}
feature-speak_position = {$verbose_coordinates ->
    [0] {$x_pos}, {$y_pos}
    *[1] X: {$x_pos}, Y: {$y_pos}
  }
feature-speak_youve_got_mail = You've got mail!
feature-speak_cheater = You're a dirty cheater!
feature-warnings-health = Warning! Health is at {$value} percent!
feature-warnings-stamina = Warning! Stamina is at {$value} percent!
feature-warnings-time = Warning! Time is {$value}
feature-grid_movement_status = Grid movement status: {$is_active ->
    [0] Inactive
    *[1] Active
  }


# Building operations

building_operations-move_building-under_construction = Cannot move building! Under construction.
building_operations-move_building-no_permission = You don't have permission to move this building!
building_operations-move_building-cannot_move = Cannot move building to {$x_position}x {$y_position}y
building_operations-move_building-building_moved = {$building_name} moved to {$x_position}x {$y_position}y
building_operations-no_building_to_demolish = No building to demolish.
building_operations-building_failed = Building failed


# FIXME update naming convention
prefix-repair = Repair {$content}

suffix-building_door = {$content} Door
suffix-map_entrance = {$content} Entrance
suffix-mill_input = {$content} Input
suffix-mill_output = {$content} Output

# Tiles

tile_name-bridge = Bridge
tile_name-boat_hull = Boat Hull
tile_name-boat_anchor = Boat Anchor
tile_name-diggable_spot = Diggable Spot
tile_name-panning_spot = Panning Spot
tile-resource_clump-large_stump-name = Large Stump
tile-resource_clump-hollow_log-name = Hollow Log
tile-resource_clump-meteorite-name = Meteorite
tile-resource_clump-boulder-name = Boulder
tile-resource_clump-mine_rock-name = Mine Rock
tile-resource_clump-giant_cauliflower-name = Giant Cauliflower
tile-resource_clump-giant_melon-name = Giant Melon
tile-resource_clump-giant_pumpkin-name = Giant Pumpkin
tile-resource_clump-giant_green_rain_weed-name = Giant Green Rain Weeds
tile-resource_clump-unknown = Unknown Resource Clump {$id}
tile-water-name = Water
tile-cooled_lava-name = Cooled Lava
tile-lava-name = Lava
tile-fertile_weed_name = Fertile weed
tile-grass-name = {$grass_type ->
    *[1] Grass
    [2] Cave grass
    [3] Frost grass
    [4] Lava grass
    [5] Cave grass
    [6] Cobweb
    [7] Blue grass
  }
tile-twig-name = {$described ->
    [0] Twig
    *[other] {$qualified_item_id ->
        [O294] Forked 
        [O295] Branching 
        *[other] {EMPTYSTRING()}
      }Twig
  }
tile-stone-name = {$described ->
    [0] {$qualified_item_id ->
        [O46] Mystic 
        [O343] {EMPTYSTRING()} 
        [O450] {EMPTYSTRING()} 
        [O668] {EMPTYSTRING()} 
        [O670] {EMPTYSTRING()} 
        [O845] {EMPTYSTRING()} 
        [O846] {EMPTYSTRING()} 
        [O847] {EMPTYSTRING()} 
        *[other] Colored 
      }Stone
    *[other] {$qualified_item_id ->
        [O32] Dull Brown Pointed Stone
        [O34] Mounded Gray Stone
        [O36] Ovular Gray Stone
        [O38] Lumpy Brown Stone
        [O40] Smooth Warm Brown Stone
        [O42] Segmented Rusty Brown Stone
        [O46] Mystic Purple Stone With Blue Swirls
        [O48] blue pointed stone
        [O50] Smooth Blue Stone
        [O52] Ovular Turquoise Stone
        [O54] Lumpy Aquamarine
        [O56] Pointed Red Stone
        [O58] Flame-Like Red Stone
        [O343] Small Gray Stone
        [O450] Extrusive Gray Stone
        [O668] Bean-Shaped Gray Stone
        [O670] Rounded Gray Stone
        [O760] Lumpy Dark Gray Stone
        [O762] Smooth Dark Gray Stone
        [O845] Bumpy Dark Purple Stone
        [O846] Mounded Dark Blue-Gray Stone
        [O847] Extrusive Dark Purple Stone
        *[other] Stone
      }
  }
tile-sprinkler-pressure_nozzle-prefix = Pressurized {$content}
tile-sprinkler-enricher-prefix = Enriching {$content}
tile-fence_gate-suffix = {$name} {$is_open ->
    [0] {$less_info ->
      [0] Closed
      *[1] {EMPTYSTRING()}
    }
    *[1] Opened
  }
tile-building_animal_door-suffix = {$name} Animal Door {$is_open ->
    [0] {$less_info ->
      [0] Closed
      *[1] {EMPTYSTRING()}
    }
    *[1] Opened
  }
tile-mine_shaft-coal_bag = Bag
tile-mine_shaft-dirt = Dirt
tile-mine_shaft-duggy_hole = Duggy Hole
tile-pet_bowl-prefix = {$is_in_use ->
    [0] Unused
    *[1] {$is_empty ->
      [0] {EMPTYSTRING()}
      *[1] Empty
    }
  } {$name}
dynamic_tile-mastery_cave-pedestal = {$has_hat ->
    [0] Empty Pedestal
    *[1] Pedestal with a Hat
  }

## Interactable Tiles

tile_name-ticket_machine = Ticket Machine
tile_name-movie_ticket_machine = Movie Ticket Machine
tile_name-missed_reward_chest = Missed Reward Chest
tile_name-traveling_cart = Traveling Cart
tile_name-traveling_cart_pig = Merchant's pig
tile_name-feeding_bench = {$is_empty ->
    [1] Empty
    *[0] {EMPTYSTRING()}
  } Feeding Bench
tile_name-special_quest_board = Special Quest Board
tile-museum_piece_showcase-suffix = {$content} Showcase
tile_name-fridge = Fridge
tile_name-mail_box = Mail Box
tile_name-mail_box-unread_mail_count-prefix = {$mail_count} {$mail_count ->
    [1] unread letter in
    *[0] unread letters in
  } {$content}
tile_name-stove = Stove
tile_name-sink = Sink
tile-railroad-witch_statue-name = Witch Statue
dynamic_tile-qi_nut_room-collection_box = Collection box
dynamic_tile-farm-grandpa_shrine = Grandpa's Shrine{SIGNOFNUMBER($candles) ->
    [positive] : {$candles} {$candles -> 
      [1] Candle
      *[other] Candles
    } lit
    *[other] {EMPTYSTRING()}
  }
tile-bundles-suffix = {$content} bundle
tile-harvestable-prefix = Harvestable {$content}
tile-busy-prefix = Busy {$content}
tile_name-stepping_stone = Stepping stone
tile_name-pathway = Pathway
tile_name-flooring = Flooring
tile-volcano_dungeon-pressure_pad = Pressure Pad
tile-volcano_dungeon-gate = Gate
tile-forest-giant_tree_sump = Giant Tree Stump

## Entrances

tile-mine_ladder-name = Ladder
tile-mine_up_ladder-name = Up Ladder
tile-mine_shaft-name = Shaft
tile-mine_elevator-name = Elevator
tile-town_festival_exit-name = Exit Festival
entrance_name-secret_woods_entrance = Secret Woods Entrance
tile-entrance = Entrance
tile-door = Door
tile-door_state-opened = Opened
tile-door_state-closed = Closed
tile-interior_door = Interior door

# Items

item_name-log = Log
item_name-magic_ink = Magic Ink
item-haley_bracelet-name = Haley's Bracelet
item-lost_book-name = Lost Book
item-suffix-book = {$content} Book
item-suffix-not_usable_here = {$content}, not usable here
item-quality_type = {$quality_index -> 
    [1] Silver
    [2] Gold
    [3] Gold
    *[4] Iridium
  } Quality
item-stamina_and_health_recovery_on_consumption = {SIGNOFNUMBER($stamina_amount) ->
    [positive] +{$stamina_amount} Energy and {SIGNOFNUMBER($health_amount) ->
        [positive] +{$health_amount} Health
        *[other] {EMPTYSTRING()}
      }
    [negative] -{$stamina_amount} Energy
    [zero] {SIGNOFNUMBER($health_amount) ->
        [positive] +{$health_amount} Health
        *[other] {EMPTYSTRING()}
      }
    *[other] {EMPTYSTRING()}
  }
item-required_item_info = Requires {$name}
item-sell_price_info = Sell Price: {$price}g
# In some cases we can't get the count of the dropped items and it is returned `0`
item-dropped_item-info = Dropped Item: {$item_count ->
    [0] {$item_name}
    [1] 1 {$item_name}
    *[other] {$item_count} {$item_name}s
  }
item-crafting_recipe_info = {$name} {$is_cooking_recipe ->
    [0] (crafting)
    *[1] (cooking)
  } {$description}
item-mastery_cave-grandpa_letter = Grandpa's Letter
item-mannequin-info = {$name}{$facing_direction ->
    [0] , Facing north
    [1] , Facing east
    [3] , Facing west
    *[2] {EMPTYSTRING()}
  }{$items_on_display ->
    [null] {EMPTYSTRING()}
    *[other] , {$items_on_display}
  }

building_name-shipping_bin = Shipping Bin
building-parrot_perch-required_nuts = Parrots require {$item_count ->
    [1] 1 nut
    *[other] {$item_count} nuts
  }
building-parrot_perch-upgrade_state_idle = Empty Parrot Perch
building-parrot_perch-upgrade_state_start_building = Parrots started building request
building-parrot_perch-upgrade_state_building = Parrots building request
building-parrot_perch-upgrade_state_complete = Request completed
building-golden_parrot = Golden Parrot

# NPCs

npc_name-old_mariner = Old Mariner
npc_name-island_trader = Island Trader
npc_name-emerald_gem_bird = Emerald Gem Bird
npc_name-aquamarine_gem_bird = Aquamarine Gem Bird
npc_name-ruby_gem_bird = Ruby Gem Bird
npc_name-amethyst_gem_bird = Amethyst Gem Bird
npc_name-topaz_gem_bird = Topaz Gem Bird
npc_name-gem_bird_common = Gem Bird
npc-farm_animal_info = {$name}, {$type}, {$age} {$age ->
    [1] month
    *[other] months
  }
npc_name-horse_with_no_name = A horse with no name
monster_name-armored = Armored {$monster_name}
monster_name-big_slime = Big {$colorful ->
    [0] {EMPTYSTRING()}
    *[1] {$color} 
  }Slime{$holding ->
    [0] {EMPTYSTRING()}
    *[1] holding {$item_name}
  }
monster_name-dangerous = Dangerous {$monster_name}
monster_name-flying_purple_shorts = Flying Purple Shorts
monster_name-mage = {$monster_name} Mage
monster_name-mutant = Mutant {$monster_name}
monster_name-slime = Slime
monster_name-truffle_crab = Truffle Crab

# Event Tiles

event_tile-egg_festival_shop-name = Egg Festival Shop
event_tile-flower_dance_shop-name = Flower Dance Shop
event_tile-soup_pot-name = Soup Pot
event_tile-luau-pierre_booth = Pierre's Booth
event_tile-spirits_eve_shop-name = Spirit's Eve Shop
event_tile-stardew_valley_fair_shop-name = Stardew Valley Fair Shop
event_tile-slingshot_game-name = Slingshot Game
event_tile-purchase_star_tokens-name = Purchase Star Tokens
event_tile-the_wheel-name = The Wheel
event_tile-fishing_challenge-name = Fishing Challenge
event_tile-fortune_teller-name = Fortune Teller
event_tile-grange_display-name = Grange Display
event_tile-strength_game-name = Strength Game
event_tile-free_burgers-name = Free Burgers
event_tile-feast_of_the_winter_star_shop-name = Feast of the Winter Star Shop

patch-trash_bear-wanted_item = {$trash_bear_name} wants {$item_name}!

# Terrain Utils

terrain_util-forage_crop_types = {$type ->
    [1] Spring onion
    [2] Ginger
    *[other] Forageable crop
  }
terrain_util-fruit_tree_growth_stage = {$stage ->
    [0] seed
    [1] sprout
    [2] sapling
    [3] bush
    *[other] tree
  }
terrain_util-tree_type = {$type ->
    [1] Oak
    [2] Maple
    [3] Pine
    [4] Oak
    [5] Maple
    [6] Palm
    [7] Mushroom
    [8] Mahogany
    [9] Palm
    [10] Bushy Green
    [11] Leafy Green
    [12] Fern Green
    [13] Mystic
    *[other] Unknown tree type number {$type}
  }
terrain_util-tree-seedling = seedling
terrain_util-tree-stump = Stump
terrain_util-tree-mossy = Mossy
terrain_util-tree_growth_stage = {$stage ->
    [1] sprout
    [2] sapling
    [3] bush
    [4] bush
    *[other] tree
  }
terrain_util-bush_type = {$type ->
    [0] Small
    [1] Medium
    [2] Large
    [3] Tea {$has_matured ->
      [0] Sapling
      *[1] Bush
    }
    [4] Golden Walnut 
    *[other] unknown
  }
terrain_util-bush = Bush
terrain_util-bush-town = Town
terrain_util-bush-greenhouse = Greenhouse
terrain_util-tent = Tent
terrain_util-tent_entrance = Tent Entrance
terrain_util-crop-watered = Watered
terrain_util-crop-unwatered = Unwatered
terrain_util-crop-dead = Dead
terrain_util-crop-soil = Soil
terrain_util-harvestable = Harvestable
terrain_util-fertilized = Fertilized


# Object Categories

object_category-animals = Animals
object_category-bridges = Bridges
object_category-buildings = Buildings
object_category-bundles = Bundles
object_category-bushes = Bushes
object_category-containers = Containers
object_category-crops = Crops
object_category-debris = Debris
object_category-decor = Decor
object_category-doors = Doors
object_category-dropped_items = Dropped items
object_category-farmers = Farmers
object_category-fishing = Fishing
object_category-fishponds = Fish Ponds
object_category-flooring = Flooring
object_category-furniture = Furniture
object_category-interactables = Interactables
object_category-machines = Machines
object_category-mine_items = Mine items
object_category-npcs = NPC's
object_category-pending = Pending
object_category-quest_items = Quest items
object_category-ready = Ready
object_category-resource_clumps = Resource clumps
object_category-trees = Trees
object_category-water = Water
object_category-unknown = Unknown
object_category-other = Other

direction-north = North
direction-south = South
direction-east = East
direction-west = West
direction-north_west = Northwest
direction-north_east = Northeast
direction-south_west = Southwest
direction-south_east = Southeast
direction-current_tile = Current tile

inventory_util-empty_slot = Empty Slot

common-unknown = Unknown

# The $name will be in the respective language i.e., it will be in french for french translation and so on. So use the language specific name in the square brackets except for the one with '*', that can have any value. Variants with '*' are marked as default.
common-util-pluralize_name = {$item_count} {PLURALIZE($item_count, $name)}
