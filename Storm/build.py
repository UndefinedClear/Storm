import subprocess
from typing import Literal

ReleaseOrDebug = Literal['Release', 'Debug']
OS_TYPES = Literal['win-x64', 'win-x32', 'linux-x64', 'linux-x32']

# String template with named placeholders
base = 'dotnet publish -c {release_type} -r {os_name} --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o {out_dir}'

def build(release_type: ReleaseOrDebug, os_name: OS_TYPES):
    # Correct way to format with multiple placeholders at once
    cmd = base.format(
        release_type=release_type,
        os_name=os_name,
        out_dir='./build'
    )

    print(f"Executing: {cmd}")
    # Using subprocess.run is safer than os.system
    try:
        subprocess.run(cmd, shell=True, check=True)
        print("Build successful!")
    except subprocess.CalledProcessError as e:
        print(f"Build failed with error: {e}")

if __name__ == "__main__":
    build('Debug', 'win-x64')
