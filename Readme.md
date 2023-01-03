# OscilloGraph

An OscilloGraph simulator, you can use it to play some special waveform files.

## Example

![Screenshot](https://github.com/ReturnNefe/OscilloGraph/blob/main/src/screenshot.png?raw=true)

## Install

1. Download and Install ``.Net 6 Runtime``

2. Download `Oscillofun.zip`

   Copy ``wave.wav`` into the root directory.

3. Run Program

    ```powershell
    ./OscilloGraph wave.wav
    ```

    It will launch an HttpServer and automatically open the browser to load the webpage.

    Click the "Start" button, and enjoy it!

## Usage

### Argument

0: file    the Wave File to play (Required)

### Options

|Option|Description|
|--|--|
| --fps <Int32>     | The default is equal to 25|
| --url <String>    | The URL that the HTTP server should use|
| --no-auto-open    | Whether to open a webpage after the program starts (default is enalbed)|
| --auto-render     | Whether to render after the webpage has loaded (default is disabled)|
| -h, --help        | Show help message|
| --version         | Show version|

## About Audio

___Version : v0.1.1 or higher___

I use ``NAudio`` to play audio, but NAudio can't work on Linux OS or macOS.

So I added a feature which allows users to choose how to play audio (``NAudio`` or ``FFMpeg``).

The default selection mode is ``auto``:

&emsp; If you use Windows OS, it will select ``NAudio``. Otherwise it will use ``FFMpeg``.

***

Also See <https://github.com/WangTingZheng/oscillofun>
