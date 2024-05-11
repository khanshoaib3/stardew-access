#!/bin/bash

# Get the changes for each category from PR message and add them to latest.md

mapfile file < ./pr_msg.txt
mapfile latest_file < ./latest.md

headings=('### New Features' '### Feature Updates' '### Bug Fixes' '### Translation Changes' '### Guides And Docs' '### Others')
indices=(-1 -1 -1 -1 -1 -1 ${#file[@]}) # 1 extra 
latest_indices=(-1 -1 -1 -1 -1 -1 ${#latest_file[@]}) # 1 extra 

# Get indices of category headings in the pr message
n=0
for line in "${file[@]}"
do
  line=$(echo "$line" | xargs) # Trim the string
  # echo $line
  for ((i = 0; i < ${#headings[@]}; i++)); do
    if [[ $line == ${headings[$i]} ]]; then
      indices[$i]=$n
    fi
  done
  ((n++))
done

# Get indices of category headings in the latest.md
n=0
for line in "${latest_file[@]}"
do
  line=$(echo "$line" | xargs) # Trim the string
  # echo $line
  for ((i = 0; i < ${#headings[@]}; i++)); do
    if [[ $line == ${headings[$i]} ]]; then
      latest_indices[$i]=$n
    fi
  done
  ((n++))
done

for ((i = 0; i < ${#latest_file[@]}; i++)); do
  latest_file[$i]=$(echo -e "${latest_file[$i]}" | sed -e 's/[[:space:]]*$//')
done

for ((i = ${#indices[@]}-2; i >= 0; i--)); do
  if [[ ${indices[$i]} == -1 ]]; then continue; fi
  for ((j = $i+1; j < ${#indices[@]}; j++)); do if [[ ${indices[$j]} != -1 ]]; then next=${indices[$j]}; break; fi ; done
  
  l=$next-${indices[$i]}-2
  changelogs=("${file[@]:${indices[$i]}+2:$l}")
  to_i=${latest_indices[$i+1]}
  ((to_i--))
  
  pp=("${latest_file[@]:0:$to_i}")
  np=("${latest_file[@]:$to_i:${#latest_file[@]}-$to_i}")
  for change_line in "${changelogs[@]}"
  do
    if [[ -z "$(echo $change_line | xargs)" ]]; then continue; fi # Skip empty lines (https://serverfault.com/a/7509)
    change_line="$(echo "$change_line" | sed -e 's/[[:space:]]*$//')" # Remove trailing whitespace
    pp+=("$change_line")
  done
  
  for change_line in "${np[@]}"; do pp+=("$change_line"); done
  
  unset latest_file
  latest_file=("${pp[@]}")
done

# printf %s\\n "${latest_file[@]}"
printf %s\\n "${latest_file[@]}" > ./latest.md
