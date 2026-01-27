import os, json
import argparse
import shutil
from datetime import datetime

TRASH_DIR = ".trash_can"

def get_versions():
    if not os.path.exists(TRASH_DIR):
        return []
    return sorted(os.listdir(TRASH_DIR))

def trash(*paths):
    if not os.path.exists(TRASH_DIR):
        os.makedirs(TRASH_DIR)

    RED = "\033[91m"
    RESET = "\033[0m"

    version = datetime.now().strftime("%Y-%m-%d_%H-%M-%S")
    version_dir = os.path.join(TRASH_DIR, version)
    os.makedirs(version_dir)

    for path in paths:
        if os.path.exists(path):
            shutil.move(path, os.path.join(version_dir, os.path.basename(path)))
            print(f"{RED}Moved '{path}' to trash{RESET} (Version: {version})")

def history():
    versions = get_versions()
    if not versions:
        print("No history available.")
        return
    
    # ANSI escape codes for coloring
    RED = "\033[91m"
    RESET = "\033[0m"
    
    print(f"{'ID':<4} | {'Version (Timestamp)':<20} | {'Files'}")
    print("-" * 70)
    
    # Reverse the order to show newest first
    # We enumerate backward to keep the IDs consistent with get_versions()
    version_list = list(enumerate(versions, 1))
    version_list.reverse()

    for i, (idx, version) in enumerate(version_list):
        v_path = os.path.join(TRASH_DIR, version)
        contents = os.listdir(v_path)
        contents_str = ", ".join(contents)
        
        line = f"{RED}{idx:<4}{RESET} | {version:<20} | {contents_str}"
        
        # Highlight the first item (newest) in red
        if i == 0:
            print(f"{line} {RED}(LAST DELETED){RESET}")
        else:
            print(line)


def restore(id_or_version, filename):
    versions = get_versions()
    target_version = None

    # Try ID mapping first
    try:
        idx = int(id_or_version) - 1
        if 0 <= idx < len(versions):
            target_version = versions[idx]
    except ValueError:
        # Not an integer, treat as timestamp string
        if id_or_version in versions:
            target_version = id_or_version

    if not target_version:
        print(f"Error: Version or ID '{id_or_version}' not found.")
        return

    src = os.path.join(TRASH_DIR, target_version, filename)
    if os.path.exists(src):
        shutil.move(src, os.path.basename(filename))
        print(f"Restored {filename} from {target_version}")
        if not os.listdir(os.path.join(TRASH_DIR, target_version)):
            os.rmdir(os.path.join(TRASH_DIR, target_version))
    else:
        print(f"Error: {filename} in version {target_version} not found.")

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="Simple trash system with versioning.")
    parser.add_argument("--history", action="store_true", help="Show trash history with IDs and contents")
    parser.add_argument("--restore", nargs=2, metavar=("ID/VERSION", "FILENAME"), help="Restore a file/directory from a specific ID or version")
    parser.add_argument("files", nargs="*", help="Files or directories to move to trash")

    args = parser.parse_args()

    if args.history:
        history()
    elif args.restore:
        restore(args.restore[0], args.restore[1])
    elif args.files:
        trash(*args.files)

