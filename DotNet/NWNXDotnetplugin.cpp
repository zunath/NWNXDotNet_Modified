/***************************************************************************
    NWNXDotnetplugin.cpp: implementation of the CNWNXDotnetplugin class.
    (c) 2005 by Ingmar Stieger (Papillon)

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 ***************************************************************************/

#include "stdafx.h"
#include "NWNXDotnetplugin.h"
#include <string> 
#using <..\NwnxAssembly\bin\release\NwnxAssembly.dll>
#using <System.Windows.Forms.dll>


CNWNXDotnetplugin::CNWNXDotnetplugin()
{
}

CNWNXDotnetplugin::~CNWNXDotnetplugin()
{
	OnRelease();
}

char* CNWNXDotnetplugin::OnRequest(char *gameObject, char* Request, char* Parameters)
{
	// This assumes you call SetLocalString like this
	// SetLocalString(oObject, "NWNX!DOTNETPLUGIN!EXECUTE", "...................................");

	long itest = 0;

	if (Request)
	{
		// This will output "EXECUTE" as request
		Log("* Got request: %s.\n", Request);
	}

	if (Parameters)
	{
		try {
		// This is the space where data can be exchanged with NWN
		Log("* Got parameters: %s.\n", Parameters);

		//the line below is a sample how to instantiate the dotnetassembly
		//	use this to hold the class in memory, to avoid internally reloading
		//	it for every call (we reload it internally so we can use Reflection)
		//	IF YOU DO THIS, don't forget to free the instantiated object when no needed anymore
		//NwnxAssembly::Execution ^e = gcnew NwnxAssembly::Execution();

		//convert to a .NET string
		System::String ^strParams = "";
		std::string s=Parameters;
		strParams = gcnew System::String(s.c_str());
		
		//execute and return
		System::String ^strReturn = NwnxAssembly::Execution::Execute(strParams);
		//char* wch = "woot";
		char* wch = (char*)System::Runtime::InteropServices::Marshal::StringToHGlobalAnsi(strReturn).ToPointer();
		strncpy(Parameters, wch, strlen (Parameters));

		// Always free the unmanaged string.
		System::Runtime::InteropServices::Marshal::FreeHGlobal(System::IntPtr(wch));
		delete strReturn;
		delete strParams;
		}
		catch (...) {

			Log("* %s.\n", "Error on executing Assembly ");
			strncpy_s(Parameters, strlen (Parameters), "Error on executing Assembly", strlen (Parameters));
		}
	}

	return NULL;
}




BOOL CNWNXDotnetplugin::OnCreate(const char* LogDir)
{
	// call the base class function
	char log[MAX_PATH];
	sprintf_s (log, "%s\\nwnx_dotnetplugin.txt", LogDir);
	if (!CNWNXBase::OnCreate(log))
		return false;

	WriteLogHeader();
	return true;
}

BOOL CNWNXDotnetplugin::OnRelease()
{
	Log("o Shutting down\n");

	// call base class function
	return CNWNXBase::OnRelease();
}

void CNWNXDotnetplugin::WriteLogHeader()
{
	fprintf(m_fFile, "NWNX Dotnet plugin V.1.3.\n");
	fprintf(m_fFile, "(c) 2005 by Cesar Ronchese \n");
	fprintf(m_fFile, "visit LOD at http://lodsite.redirectme.net\n");
	fprintf(m_fFile, "visit NWNX (by Pappilon) at http://www.nwnx.org\n\n");
}