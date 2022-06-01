Q			:=@
CC			:=g++ -std=c++11 -Wall -Wextra
SRCS		:=$(wildcard *.cpp)
OBJECTS		:=
TARGET		:=iCAP_CoreService_CM
CPPFLAGS	:=-I./include
CPPFLAGS	+=-I/usr/local/include
CPPFLAGS	+=-I../../../../../../../trunk/library/libcrypto/bin/include
LIBS		:=../../../../../../../trunk/library/libcrypto/bin/libcrypto_64.a
LIBS		+=-lhasp_cpp_linux_x86_64
LIBS		+=libhasp_linux_x86_64_105114.a
LIBS		+=-lcrypto -lpthread
LDFLAGS		:=-L./
MACROS		:=

.PHONY: all
all: clean $(TARGET)

$(TARGET):
	$(Q)echo "  Building '$@' ..."
	$(Q)$(CC) $(SRCS) -o $@ $(CPPFLAGS) $(LDFLAGS) $(LIBS) $(MACROS)

.PHONY: clean
clean:
	$(Q)echo "  Cleaning '$(TARGET)' ..."
	$(Q)$(RM) $(TARGET)