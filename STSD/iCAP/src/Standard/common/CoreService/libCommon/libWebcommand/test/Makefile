Q			=@
SRCDIR  	=../src
INCLUDEDIR	=$(SRCDIR)/include
OBJDIR  	=../obj
BINDIR  	=../bin
TARGET		=unit_test
CXX			?=g++
MACROS		:=
CPPFLAGS	:=$(MACROS)
CXXFLAGS	:=-std=c++11 -Wall -Wextra
CXXFLAGS	+=-I$(INCLUDEDIR)
LDFLAGS		:=-L../../../../../../../../../../trunk/library/liblogger/bin
LDFLAGS		+=-L/usr/local/lib
LIBS		:=$(OBJDIR)/lib_WebCommand_64.a
LIBS		+=-lcurl -lcurlpp -lgtest -lgtest_main -ljsoncpp -pthread
LIBS		+=-llogger -lboost_date_time -lboost_filesystem -lboost_log -lboost_log_setup -lboost_system -lboost_thread
SOURCES		:=$(wildcard *.cpp)
INCLUDES	:=$(wildcard $(INCLUDEDIR)/*.hpp)
OBJECTS		:=$(SOURCES:%.cpp=%.o)
RM        	:=rm -f

.PHONY: all
all: clean $(TARGET) unittest

$(TARGET): $(OBJECTS)
	$(Q)$(CXX) $^ -o $@ $(LDFLAGS) $(LIBS)
	$(Q)echo "  Linking '$@' complete!"

$(OBJECTS): %.o : %.cpp
	$(Q)$(CXX) $(CPPFLAGS) $(CXXFLAGS) -o $@ -c $<
	$(Q)echo "  Compiled '$<' successfully."

.PHONY: unittest
unittest:
	$(Q)echo "  Start 64-bit unit tests ..."
	$(Q)./unit_test
	$(Q)echo "  End of 64-bit unit tests ..."

.PHONY: clean
clean:
	$(Q)echo "  Cleaning '$(TARGET)' ..."
	$(Q)$(RM) $(TARGET)
	$(Q)$(RM) $(OBJECTS)