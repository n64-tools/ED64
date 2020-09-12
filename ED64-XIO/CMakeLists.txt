cmake_minimum_required(VERSION 3.15)

include(CMakeToolsHelpers OPTIONAL)
include(ExternalProject)

set(CMAKE_VERBOSE_MAKEFILE ON CACHE BOOL "ON" FORCE) # debug helper

# the following prevents launching a build in the source tree
set(CMAKE_DISABLE_SOURCE_CHANGES ON)
set(CMAKE_DISABLE_IN_SOURCE_BUILD ON)

message(STATUS "Building for hardware: ${HARDWARE_TYPE} v${HARDWARE_REVISION}")

project(OS64 #rename for your ROM name
		VERSION ${BUILD_VERSION}
		LANGUAGES C ASM)


set( ELF_NAME 			${PROJECT_NAME}.elf )
set( BIN_NAME 			${PROJECT_NAME}.bin )
set( ROM_NAME 			${PROJECT_NAME}.v64 )

set( LINKER_FILE_NAME "${LIBDRAGON_PREFIX}/ldscripts/ed64.ld" )
set(ROM_HEADER ${LIBDRAGON_PREFIX}/headers/header)

# make sure the version is set
if(NOT BUILD_VERSION)
	set(BUILD_VERSION 3.14.0)
endif()

string(REPLACE "." ";" VERSION_LIST ${BUILD_VERSION})
list(GET VERSION_LIST 3 BUILD_VER_MAJOR)
list(GET VERSION_LIST 4 BUILD_VER_MINOR)
list(GET VERSION_LIST 1 BUILD_VER_REV)



add_executable( ${ELF_NAME}
)

target_compile_definitions( ${ELF_NAME} PUBLIC
	OS_VER_MAJOR=${BUILD_VER_MAJOR}
	OS_VER_MINOR=${BUILD_VER_MINOR}
	OS_VER_REV=${BUILD_VER_REV}
	OS_TARGET_TYPE=${HARDWARE_TYPE}
	OS_TARGET_REV=${HARDWARE_REVISION}
)

target_sources( ${ELF_NAME} PUBLIC
	${CMAKE_CURRENT_LIST_DIR}/src/bios.c
	${CMAKE_CURRENT_LIST_DIR}/src/disk.c
	${CMAKE_CURRENT_LIST_DIR}/src/fmanager.c
	${CMAKE_CURRENT_LIST_DIR}/src/incs.s
	${CMAKE_CURRENT_LIST_DIR}/src/main.c
	${CMAKE_CURRENT_LIST_DIR}/src/sys.c
	${CMAKE_CURRENT_LIST_DIR}/ff/diskio.c
	${CMAKE_CURRENT_LIST_DIR}/ff/ff.c
	${CMAKE_CURRENT_LIST_DIR}/ff/ffsystem.c
	${CMAKE_CURRENT_LIST_DIR}/ff/ffunicode.c
	${CMAKE_CURRENT_LIST_DIR}/ff/fileio.c
	${CMAKE_CURRENT_LIST_DIR}/ff/usb.c
)

target_include_directories( ${ELF_NAME}
	PUBLIC
	${CMAKE_CURRENT_LIST_DIR}/inc
	${CMAKE_CURRENT_LIST_DIR}/ff
)

target_link_directories( ${ELF_NAME}
	PUBLIC
	${CMAKE_CURRENT_LIST_DIR}/lib
)

set_target_properties( ${ELF_NAME} 
	PROPERTIES LINK_FLAGS "-T ${LINKER_FILE_NAME}"
	) 

add_custom_command(TARGET ${ELF_NAME}
                  POST_BUILD
                  COMMAND ${CMAKE_OBJCOPY} ARGS -O binary ${ELF_NAME} ${BIN_NAME}
				  )
				  	
add_custom_command(TARGET ${ELF_NAME}
                  POST_BUILD
                  COMMAND ${N64_TOOL} ARGS -l 1M -t "${PROJECT_NAME}" -h ${ROM_HEADER} -o ${ROM_NAME} ${BIN_NAME}
				  )
				  
add_custom_command(TARGET ${ELF_NAME}
                  POST_BUILD
                  COMMAND ${CHECKSUM_TOOL} ARGS ${ROM_NAME}
				  )

if(DEPLOY)
	separate_arguments(DEPLOY_EXPORT_PARAMETERS_LIST WINDOWS_COMMAND "${DEPLOY_PARAMETERS}")
	add_custom_command(TARGET ${ELF_NAME}
						POST_BUILD
						COMMAND ${DEPLOY} ${DEPLOY_EXPORT_PARAMETERS_LIST}
						)
endif(DEPLOY)
