#!/usr/bin/bash

set -e
pacman -S --noconfirm --needed git mingw-w64-x86_64-toolchain mingw-w64-x86_64-lld mingw-w64-x86_64-cmake msys/tar msys/make mingw-w64-x86_64-sqlite3 mingw64/mingw-w64-x86_64-gstreamer mingw64/mingw-w64-x86_64-gst-plugins-good mingw64/mingw-w64-x86_64-gst-plugins-base mingw64/mingw-w64-x86_64-gst-plugins-ugly mingw64/mingw-w64-x86_64-gst-plugins-bad mingw64/mingw-w64-x86_64-faad2 mingw64/mingw-w64-x86_64-portaudio
