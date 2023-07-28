building_operations-move_building-under_construction = Cannot move building! Under construction.
building_operations-move_building-no_permission = You don't have permission to move this building!
building_operations-move_building-cannot_move = Cannot move building to {$x_position}x {$y_position}y
building_operations-move_building-building_moved = {$building_name} moved to {$x_position}x {$y_position}y

# Menus

## Bundle Menus

### Common

menu-bundle-completed-prefix = Completed {$content}

### JoJa CD Menu

menu-joja_cd-project_info = {$name}, Cost: {$price}g, Description: {$description}
menu-joja_cd-project_name = {$project_index ->
    [0] Bus
    [1] Minecarts
    [2] Bridge
    [3] Greenhouse
    [4] Panning
    *[other] Unknown
  } Project

### Junimo Note Menu

menu-junimo_note-scrambled_text = Scrambled text
menu-junimo_note-current_area_info-prefix = Area: {$area_name}, Reward: {$completion_reward}, 
menu-junimo_note-bundle_open_button = {$bundle_name} bundle
menu-junimo_note-input_slot = Input Slot {$index}
menu-junimo_note-collect_rewards = Collect rewards
menu-junimo_note-next_area_button = Next area button
menu-junimo_note-previous_area_button = Previous area button
menu-junimo_note-back_button = Back button
menu-junimo_note-purchase_button = Purchase button

## Animal Query Menu (TODO organise this!)
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

## Inventory Menu

menu-inventory-empty_slot-name = Empty Slot
menu-inventory-not_usable-suffix = , not usable here

## Number Selection Menu

menu-number_selection-button-left_button = Decrease value button
menu-number_selection-button-right_button = Increase value button
menu-number_selection-value_and_price_info = {$value} {$price ->
    [0] {EMPTYSTRING()}
    *[other] Price: {$price}
  }


# FIXME update naming convention
prefix-repair = Repair {$content}

suffix-building_door = {$content} Door
suffix-building_animal_door = {$content } Animal Door {$is_open ->
    [0] Closed
    *[1] Opened
  }
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
tile-water-name = Water
tile-cooled_lava-name = Cooled Lava
tile-lava-name = Lava
tile-grass-name = Grass
tile-sprinkler-pressure_nozzle-prefix = Pressurized {$content}
tile-sprinkler-enricher-prefix = Enriching {$content}

## Interactable Tiles

tile_name-ticket_machine = Ticket Machine
tile_name-movie_ticket_machine = Movie Ticket Machine
tile_name-missed_reward_chest = Missed Reward Chest
tile_name-traveling_cart = Traveling Cart
tile_name-feeding_bench = {$is_empty ->
    [1] Empty
    *[0] {EMPTYSTRING()}
  } Feeding Bench
tile_name-special_quest_board = Special Quest Board
tile-museum_piece_showcase-suffix = {$content} Showcase
tile_name-fridge = Fridge
tile_name-mail_box = Mail Box
tile_name-stove = Stove
tile_name-sink = Sink
tile-railroad-witch_statue-name = Witch Statue

## Entrances

tile-mine_ladder-name = Ladder
tile-mine_up_ladder-name = Up Ladder
tile-mine_shaft-name = Shaft
tile-mine_elevator-name = Elevator
tile-town_festival_exit-name = Exit Festival

#---------------------------------#

# Items

item_name-log = Log
item_name-magic_ink = Magic Ink
item-haley_bracelet-name = Haley's Bracelet
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
item-dropped_item-info = Dropped Item: {$item_count ->
    [1] 1 {$item_name}
    *[other] {$item_count} {$item_name}s
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

entrance_name-secret_woods_entrance = Secret Woods Entrance

feature-speak_selected_slot_item_name = {$slot_item_name} Selected
feature-speak_location_name = {$location_name} Entered
feature-read_tile-manually_triggered_info = {$tile_name}, Category: {$tile_category}
feature-speak_health_n_stamina-in_percentage_format = Health is {$health} % and Stamina is {$stamina} %
feature-speak_health_n_stamina-in_normal_format = Health is {$health} and Stamina is {$stamina}
feature-warnings-health = Warning! Health is at {$value} percent!
feature-warnings-stamina = Warning! Stamina is at {$value} percent!
feature-warnings-time = Warning! Time is {$value}

npc_name-old_mariner = Old Mariner
npc_name-island_trader = Island Trader
npc_name-emerald_gem_bird = Emerald Gem Bird
npc_name-aquamarine_gem_bird = Aquamarine Gem Bird
npc_name-ruby_gem_bird = Ruby Gem Bird
npc_name-amethyst_gem_bird = Amethyst Gem Bird
npc_name-topaz_gem_bird = Topaz Gem Bird
npc_name-gem_bird_common = Gem Bird

# Event Tiles

event_tile-egg_festival_shop-name = Egg Festival Shop
event_tile-flower_dance_shop-name = Flower Dance Shop
event_tile-soup_pot-name = Soup Pot
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
event_tile-travelling_cart-name = Travelling Cart
event_tile-feast_of_the_winter_star_shop-name = Feast of the Winter Star Shop

# Copied from default.json (needs to be organised)

grandpastory-scene0 = Grandpa, on his deathbed.
grandpastory-scene4 = Employees working in JoJa corp.
grandpastory-scene5 = Employees in their cubicles, some of them look exhausted including yourself.
grandpastory-scene6 = You reach your desk finding grandpa's letter.
grandpastory-letteropen = Left click to open grandpa's letter
intro-scene3 = Traveling to Stardew Valley bus stop
intro-scene4 = Stardew valley 0.5 miles away
patch-trash_bear-wanted_item = {$trash_bear_name} wants {$item_name}!


common-unknown = Unknown

# Common UI elements
common-ui-ok_button = OK button
common-ui-cancel_button = Cancel button

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
