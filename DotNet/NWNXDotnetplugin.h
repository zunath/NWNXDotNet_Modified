/***************************************************************************
    NWNXDotnetplugin.h: interface for the CNWNXDotnetplugin class.
    (c) 2005 Ingmar Stieger (Papillon)

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

#if !defined(DOTNETPLUGIN_H_)
#define DOTNETPLUGIN_H_

#include "..\NWNXDLL\NWNXBase.h"

class CNWNXDotnetplugin : public CNWNXBase  
{
public:
	CNWNXDotnetplugin();
	virtual ~CNWNXDotnetplugin();

	char* OnRequest(char *gameObject, char* Request, char* Parameters);
	BOOL OnCreate(const char* LogDir);
	BOOL OnRelease();

	void WriteLogHeader();
};

#endif

