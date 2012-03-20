#  ======================================================================================
#  File         : Makefile
#  Author       : Wu Jie 
#  Last Change  : 09/10/2011 | 15:00:32 PM | Saturday,September
#  Description  : 
#  ======================================================================================

# /////////////////////////////////////////////////////////////////////////////
#  general
# /////////////////////////////////////////////////////////////////////////////

# settings (windows)
# UNITY_PATH = $(subst \,/,$(PROGRAMFILES))/Unity/Editor/Data
# settings (mac)
UNITY_PATH = /Applications/Unity/Unity.app/Contents/Frameworks

# unit essential (windows)
# COMPILER = $(UNITY_PATH)/Mono/bin/gmcs.bat
# unit essential (mac)
COMPILER = $(UNITY_PATH)/Mono/bin/gmcs

UNITY_ENGINE_DLL = $(UNITY_PATH)/Managed/UnityEngine.dll
UNITY_VER = UNITY_3_5 

# Utilities.
MKDIR = mkdir -p
RM = rm -f

# Target
TARGET_DIR = build
RUNTIME_TARGET = $(TARGET_DIR)/ex2D.Runtime.dll

# /////////////////////////////////////////////////////////////////////////////
# do build
# /////////////////////////////////////////////////////////////////////////////

.PHONY: clean rebuild

all: $(RUNTIME_TARGET)
clean: 
	$(RM) $(RUNTIME_TARGET)
rebuild: |clean all

# /////////////////////////////////////////////////////////////////////////////
#  runtime target
# /////////////////////////////////////////////////////////////////////////////

# get sources 
RUNTIME_SOURCE_DIRS += ex2D/Core/Asset/
RUNTIME_SOURCE_DIRS += ex2D/Core/Component/
RUNTIME_SOURCE_DIRS += ex2D/Core/Component/AnimationHelper/
RUNTIME_SOURCE_DIRS += ex2D/Core/Component/Camera/
RUNTIME_SOURCE_DIRS += ex2D/Core/Component/Helper/
RUNTIME_SOURCE_DIRS += ex2D/Core/Component/Manager/
RUNTIME_SOURCE_DIRS += ex2D/Core/Component/Sprite/
RUNTIME_SOURCE_DIRS += ex2D/Core/Extension/
RUNTIME_SOURCE_DIRS += ex2D/Core/Helper/
RUNTIME_SOURCE = $(wildcard $(addsuffix *.cs,$(RUNTIME_SOURCE_DIRS)))

# defines
RUNTIME_DEFINE = -d:UNITY_3_5

# deubg argument
# RUNTIME_ARGUMENT = $(RUNTIME_DEFINE) -d:DEBUG -r:$(UNITY_ENGINE_DLL)
# release argument
RUNTIME_ARGUMENT = $(RUNTIME_DEFINE) -r:$(UNITY_ENGINE_DLL)

# do the build
$(RUNTIME_TARGET):
	$(MKDIR) $(TARGET_DIR)
	$(COMPILER) -target:library -out:$(RUNTIME_TARGET) $(RUNTIME_ARGUMENT) $(RUNTIME_SOURCE)

