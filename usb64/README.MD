# PC Program for communicating with the ED64 X-Series OS via USB
OS version 3.04 (or Unofficial OS 2.8.0.9) and above.

## supported commands:
### FPGA
Loads an RBF file into the ED64's fpga
Arg: `-fpga="<filename>"`


### Transfer ROM
Writes a (ROM) file to ED64's volatile memory
Arg: `-rom="<filename>"`


### Start ROM
Starts the ROM written to the ED64's volatile memory
Arg: `-start`		 


### Perform diagnostics
Compares that a write and read from the ED64's volatile memory matches.
Arg: `-diag`


### Dump ROM
Dumps the loaded ROM from the ED64's volatile memory
Arg: `-drom="<filename>"`


### Take screenshot
Generates an image from the frame buffer (in bitmap format)
Arg `-screen="<filename>"`

### Get Real Time Clock
Gets the current time set on the ED64
Arg `-getrtc`

### Set Real Time Clock
Sets the current time set on the ED64
Arg `-setrtc="yyyy-mm-ddThh:mm:ssZ"`
