patch-animal_query_menu-heart =
    {$name}, {$type}, {$heart_count ->
    [0] 0 hearts
    [1] 1 heart
    *[other] {$heart_count} hearts
    }, {$age} old, {$parent_name ->
        [null] .
        *[other] Parent Name is {$parent_name}.
    }
