/*
TC Plyer
Total Commander Audio Player plugin & standalone player written in C#, based on bass.dll components
Copyright (C) 2016 Webmaster442 aka. Ruzsinszki Gábor

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
namespace TCPluginInstaller
{
    /// <summary>
    /// Plugin types
    /// </summary>
    internal enum PluginType
    {
        /// <summary>
        /// Packer WCX
        /// </summary>
        Packer,
        /// <summary>
        /// Lister WLX
        /// </summary>
        Lister,
    }
}