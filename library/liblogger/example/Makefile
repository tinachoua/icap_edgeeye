Q			=@
TARGET		=example
CXX			?=g++
CPPFLAGS	:=-DBOOST_LOG_DYN_LINK
CXXFLAGS	:=-Wall -Wextra -ggdb -I../bin/include
LDFLAGS		:=-L../bin -L/usr/local/lib -L/usr/lib/x86_64-linux-gnu
LIBS		:=-llogger -lboost_date_time -lboost_filesystem -lboost_log -lboost_log_setup 
LIBS		+=-lboost_system -lboost_thread -pthread
SOURCES		:=$(wildcard *.cpp)
OBJECTS		:=$(SOURCES:%.cpp=%.o)
RM        	:=rm -f

.PHONEY: all
all: clean $(TARGET)

$(TARGET): $(OBJECTS)
	$(Q)$(CXX) $^ -o $@ $(LDFLAGS) $(LIBS)
	$(Q)echo "Linking complete!"

$(OBJECTS): %.o : %.cpp
	$(Q)$(CXX) $(CPPFLAGS) $(CXXFLAGS) -o $@ -c $<
	$(Q)echo "  Compiled "$<" successfully!"

.PHONEY: clean
clean:
	$(Q)$(RM) $(OBJECTS)
	$(Q)$(RM) $(TARGET)
	$(Q)echo "  Cleanup complete!"
