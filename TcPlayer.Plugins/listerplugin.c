#include <Windows.h>
#include <cwchar>
#include <Shlwapi.h>
#include "listplug.h"

#define EXTENSIONS "MULTIMEDIA & (EXT=\"MP3\" | EXT=\"MP3PRO\" | EXT=\"MP1\" | EXT=\"MP2\" | EXT=\"M4A\" | EXT=\"M4B\" | EXT=\"AAC\" | EXT=\"FLAC\" | EXT=\"AC3\" | EXT=\"WV\" | EXT=\"WAV\" | EXT=\"AIFF\" | EXT=\"AIF\" | EXT=\"WMA\" | EXT=\"MIDI\" | EXT=\"MID\" | EXT=\"RMI\" | EXT=\"KAR\" | EXT=\"OGG\" | EXT=\"MOD\" | EXT=\"XM\" | EXT=\"IT\" | EXT=\"S3M\" | EXT=\"MTM\" | EXT=\"UMX\" | EXT=\"MO3\" | EXT=\"M3U\" | EXT=\"PLS\" | EXT=\"WPL\" | EXT=\"APE\" | EXT=\"MPC\" | EXT=\"MP+\" | EXT=\"MPP\" | EXT=\"OFR\" | EXT=\"OFS\" | EXT=\"SPX\" | EXT=\"TTA\" | EXT=\"DSF\" | EXT=\"DSDIFF\" | EXT=\"OPUS\")"
#define PROGRAMNAME L"TCPlayer.exe"

HINSTANCE hinst;


BOOL APIENTRY DllMain(HANDLE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved)
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
		hinst = (HINSTANCE)hModule;
		break;
	case DLL_PROCESS_DETACH:
		break;
	case DLL_THREAD_ATTACH:
		break;
	case DLL_THREAD_DETACH:
		break;
	}
	return TRUE;
}