#!/usr/bin/env sh
set -eu
cd "$(dirname "$0")"
: "${GODOT_NET:?Set GODOT_NET to the Godot 4.5 .NET executable}"
dotnet build Shuabao.csproj
"$GODOT_NET" --headless --path . --editor --import
exec "$GODOT_NET" --path .
