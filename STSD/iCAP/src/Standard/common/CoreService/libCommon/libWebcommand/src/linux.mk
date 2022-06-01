Q			=@
SRCDIR  	=
INCLUDEDIR	=include
OBJDIR  	=../obj
BINDIR  	=../bin
TARGET		=$(OBJDIR)/lib_WebCommand_64.a
TARGETHADER	=$(INCLUDEDIR)/WebCommand.hpp
CXX			?=g++
MACRO		:=
CPPFLAGS	:=-DBOOST_LOG_DYN_LINK $(MACRO)
CXXFLAGS	:=-std=c++11 -Wall -Wextra -I$(INCLUDEDIR)
CXXFLAGS	+=-I../../../../../../../../../library/liblogger/bin/include
SOURCES		:=$(wildcard *.cpp)
INCLUDES	:=$(wildcard $(INCLUDEDIR)/*.hpp)
OBJECTS		:=$(SOURCES:%.cpp=$(OBJDIR)/%.o64)
RM        	:=rm -r -f

.PHONEY: all
all: clean gendir $(TARGET) install

$(TARGET): $(OBJECTS)
	$(Q)echo "  Building '$@' ..."
	$(Q)ar rcs -o $@ $(OBJECTS)

$(OBJECTS): $(OBJDIR)/%.o64 : %.cpp
	$(Q)$(CXX) $(CPPFLAGS) $(CXXFLAGS) -o $@ -c $<
	$(Q)echo "  Compiled "$<" successfully!"

.PHONEY: gendir
gendir:
	$(Q)mkdir -p $(OBJDIR)

.PHONY: install
install:
	$(Q)mkdir -p $(BINDIR)/include
	$(Q)cp $(TARGET) $(BINDIR) 
	$(Q)cp $(TARGETHADER) $(BINDIR)/include
	$(Q)echo "  Install complete!"

.PHONEY: clean
clean:
	$(Q)$(RM) $(OBJDIR)
	$(Q)$(RM) $(BINDIR)
	$(Q)echo "  Cleanup complete!"

.PHONEY: unittest
unitest:
	$(Q)cd ../test && $(MAKE)