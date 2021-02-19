
#HEADERPATH = $(ROOTDIR)/mips64-elf/lib
#HEADERNAME = header
#LINK_FLAGS = -L$(ROOTDIR)/mips64-elf/lib -led64 -ldragon -lc -lm -ldragonsys -Tn64.ld
#CFLAGS = -std=gnu99 -march=vr4300 -mtune=vr4300 -O2 -Wall -I$(ROOTDIR)/mips64-elf/include


ROOTDIR = $(N64_INST) #ROOTDIR = E:/devtools/mips64
GCCN64PREFIX = $(ROOTDIR)/bin/mips64-elf-
CHKSUM64PATH = $(ROOTDIR)/bin/chksum64
MKDFSPATH =	$(ROOTDIR)/bin/mkdfs
N64TOOL = $(ROOTDIR)/bin/n64tool
FFDIR :=ff
SRCDIR :=src
INCDIR :=inc
RESDIR :=res
LDDIR :=ldscripts
HDDIR :=headers
#ROMFIX64 =	./tools/romfix64
LINK_FLAGS = -L$(ROOTDIR)/lib -L$(ROOTDIR)/mips64-elf/lib -ldragon -lc -lm -lnosys -T $(LDDIR)/ed64.ld
PROG_NAME =	ED64-XIO-SAMPLE
FLAGS = -std=gnu99 -march=vr4300 -mtune=vr4300 -O2 -Wall -Wno-pointer-sign -I$(ROOTDIR)/mips64-elf/include -Iedlib -I$(FFDIR) -I$(INCDIR) -G0 
FLAGS_VT = -std=gnu99 -march=vr4300 -mtune=vr4300 -O0 -Wall -Wno-pointer-sign -I$(ROOTDIR)/mips64-elf/include -Iedlib -I$(FFDIR) -I$(INCDIR)
ASFLAGS = -mtune=vr4300 -march=vr4300
CC = $(GCCN64PREFIX)gcc
AS = $(GCCN64PREFIX)as
LD = $(GCCN64PREFIX)ld
OBJCOPY = $(GCCN64PREFIX)objcopy


BDIR		:=bin
ODIR		:=obj
#VPATH		:= $(BDIR)

MAKEFLAGS= -j8


SRC = $(wildcard $(SRCDIR)/*.c) $(wildcard $(FFDIR)/*.c)
SRCS = $(wildcard $(SRCDIR)/*.s) 
OBJ = $(SRC:.c=.o)
OBJ += $(SRCS:.s=.o)
OFILES = $(OBJ:%.o=obj/%.o)


all: $(PROG_NAME).v64


$(PROG_NAME).v64: $(PROG_NAME).elf
	$(OBJCOPY) $(BDIR)/$(PROG_NAME).elf $(BDIR)/$(PROG_NAME).bin -O binary
	$(RM) -f $(PROG_NAME).v64
	$(N64TOOL) -l 256K -t $(PROG_NAME) -h $(HDDIR)/header -o $(PROG_NAME).v64 $(BDIR)/$(PROG_NAME).bin -s
	$(ROMFIX64) $(PROG_NAME).v64

$(PROG_NAME).elf : $(OBJ)
	$(LD) -o $(BDIR)/$(PROG_NAME).elf $(OFILES) $(LINK_FLAGS)

%.o: %.c
	$(CC) $(FLAGS) -c $< -o $(ODIR)/$@

%.o: %.s
	$(AS) $(ASFLAGS) $< -o $(ODIR)/$@

clean:
	$(RM) -f *.v64 *.elf *.o
	$(RM) $(ODIR)/*.o
	$(RM) $(BDIR)/*.elf
	$(RM) $(BDIR)/*.bin
	$(RM) $(ODIR)/$(FFDIR)/*.o
	