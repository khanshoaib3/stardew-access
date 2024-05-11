# Copies the changelogs from one file to another

import argparse
import os


headings = ['### New Features', '### Feature Updates', '### Bug Fixes',
            '### Translation Changes', '### Guides And Docs', '### Others']


def main():
    source_path, dest_path = get_file_names_from_cli()

    src_file = open(source_path, "r").readlines()
    dest_file = open(dest_path, "r").readlines()

    print("Source file contents:")
    print(*src_file)

    print("Destination file contents:")
    print(*dest_file)

    src_file = [line.rstrip() for line in src_file]  # Remove trailing spaces
    dest_file = [line.rstrip() for line in dest_file]  # Remove trailing spaces

    for heading in headings:
        if (heading not in src_file):
            continue
        print(f"Heading ({heading}) found...")

        change_logs = get_changelogs_for_heading_in_file(heading, src_file)

        if heading in dest_file:
            insert_index = get_next_heading_index_in_file(dest_file.index(heading), dest_file) - 1
        else:
            # Heading's not preset in the destination file
            change_logs = [heading, ''] + change_logs + ['']
            for j in range(headings.index(heading)+1, len(headings)):
                if headings[j] in dest_file:
                    insert_index = dest_file.index(headings[j])
                    break
            else:
                insert_index = len(dest_file)
            print(f"Heading not found in destination file, the assumed insert index is {insert_index}")

        print(f"Inserting following lines at index {insert_index}:\n\t{change_logs}")
        dest_file = dest_file[:insert_index] + change_logs + dest_file[insert_index:]

    dest_file_w = open(dest_path, "w")
    dest_file = [line+"\n" for line in dest_file]  # Add trailing line break
    print("\n\nOverwriting destination file with the content:")
    print(*dest_file)
    dest_file_w.writelines(dest_file)


def get_changelogs_for_heading_in_file(heading: str, file_contents: list) -> list:
    heading_index = file_contents.index(heading)
    next_heading_index = get_next_heading_index_in_file(heading_index, file_contents)

    change_logs = file_contents[heading_index + 1: next_heading_index]
    change_logs = list(filter(lambda x: x.strip() != '', change_logs))  # Remove empty lines

    return change_logs


def get_next_heading_index_in_file(current_heading_index: int, file_contents: list) -> int:
    next_heading_index = len(file_contents)
    for line in file_contents[current_heading_index+1:]:
        if line in headings:
            next_heading_index = file_contents.index(line)
            break
    return next_heading_index


def get_file_names_from_cli():
    parser = argparse.ArgumentParser()
    parser.add_argument(
        "source", help='The file path from which the changelogs are to be extracted.')
    parser.add_argument(
        "destination", help='The file path to which the changelogs are to be written.')

    parsed_args = parser.parse_args()
    if not os.path.exists(parsed_args.source):
        print(f"File `{parsed_args.source}` not found!")
        exit(0)

    if not os.path.exists(parsed_args.destination):
        print(f"File `{parsed_args.destination}` not found!")
        exit(0)

    return [parsed_args.source, parsed_args.destination]


if __name__ == "__main__":
    main()
