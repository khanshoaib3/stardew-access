building_operations-move_building-under_construction = Cannot move building! Under construction.
building_operations-move_building-no_permission = You don't have permission to move this building!
building_operations-move_building-cannot_move = Cannot move building to {$x_position}x {$y_position}y
building_operations-move_building-building_moved = {$building_name} moved to {$x_position}x {$y_position}y

# Related to animal query menu
animal_query_menu-animal_info =
	{$name}, {$is_baby ->
		[0] {$type}
		*[1] Baby {$type}
	}, {$heart_count ->
		[1] 1 heart
		*[other] {$heart_count} hearts
	}, {$age ->
		[1] 1 month
		*[other] {$age} months
	} old, {$parent_name ->
		[null] {EMPTYSTRING()}
		*[other] Parent: {$parent_name}.
	}
animal_query_menu-ui-confirm_selling_button = Confirm selling animal button
animal_query_menu-ui-cancel_selling_button = Cancel selling animal button
animal_query_menu-ui-selling_button = Sell for {$price}g button
animal_query_menu-ui-move_home_button = Change home building button
animal_query_menu-ui-text_box = Animal name text box
animal_query_menu-ui-allow_reproduction_button =
	{$checkbox_value ->
		[0] Disabled
		*[1] Enabled
	} allow pregnancy button

menu-inventory-empty_slot-name = Empty Slot


# FIXME update naming convention
prefix-repair = Repair {$content}

suffix-building_door = {$content} Door
suffix-building_animal_door = {$content } Animal Door {$is_open ->
		[0] Closed
		*[1] Opened
	}
suffix-mill_input = {$content} Input
suffix-mill_output = {$content} Output

tile_name-bridge = Bridge
tile_name-ticket_machine = Ticket Machine
tile_name-movie_ticket_machine = Movie Ticket Machine
tile_name-boat_hull = Boat Hull
tile_name-boat_anchor = Boat Anchor
tile_name-missed_reward_chest = Missed Reward Chest
tile_name-mail_box = Mail Box
tile_name-stove = Stove
tile_name-sink = Sink
tile_name-fridge = Fridge
tile_name-traveling_cart = Traveling Cart
tile_name-cooled_lava = Cooled Lava
tile_name-lava = Lava
tile_name-diggable_spot = Diggable Spot
tile_name-panning_spot = Panning Spot
tile_name-feeding_bench = Feeding Bench
tile_name-feeding_bench_empty = Empty Feeding Bench
tile_name-special_quest_board = Special Quest Board
tile-museum_piece_showcase-suffix = {$content} Showcase

item_name-log = Log
item_name-magic_ink = Magic Ink
item-lost_book-name = Lost Book
item-suffix-book = {$content} Book
item-suffix-not_usable_here = {$content} not usable here
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

building_name-shipping_bin = Shipping Bin
building-parrot_perch-required_nuts = Parrots require {$item_count ->
		[1] 1 nut
		*[other] {$item_count} nuts
	}
building-parrot_perch-upgrade_state_idle = Empty Parrot Perch
building-parrot_perch-upgrade_state_start_building = Parrots started building request
building-parrot_perch-upgrade_state_building = Parrots building request
building-parrot_perch-upgrade_state_complete = Request completed

entrance_name-secret_woods_entrance = Secret Woods Entrance

feature-speak_selected_slot_item_name = {$slot_item_name} Selected
feature-speak_location_name = {$location_name} Entered

npc_name-old_mariner = Old Mariner
npc_name-island_trader = Island Trader
npc_name-emerald_gem_bird = Emerald Gem Bird
npc_name-aquamarine_gem_bird = Aquamarine Gem Bird
npc_name-ruby_gem_bird = Ruby Gem Bird
npc_name-amethyst_gem_bird = Amethyst Gem Bird
npc_name-topaz_gem_bird = Topaz Gem Bird
npc_name-gem_bird_common = Gem Bird


# Common UI elements
common-ui-ok_button = OK button

# The $name will be in the respective language i.e., it will be in french for french translation and so on. So use the language specific name in the square brackets except for the one with '*', that can have any value. Variants with '*' are marked as default.
# TODO add this to more places
common-util-pluralize_name = 
	{$name ->
		[Quartz] {$item_count ->
			[1] Quartz
			*[other] {$item_count} Quartz
		}
		[Topaz] {$item_count ->
			[1] Topaz
			*[other] {$item_count} Topazes
		}
		[Strawberry] {$item_count ->
			[1] Strawberry
			*[other] {$item_count} Strawberries
		}
		*[other] {$item_count ->
			[1] {$name}
			*[other] {$item_count} {$name}s
		}
	}
