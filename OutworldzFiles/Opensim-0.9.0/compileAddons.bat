cd addon-modules
git clone https://github.com/JakDaniels/OpenSimBirds.git
cd ..
call runprebuild.bat
call compile.bat
cd bin

@rem Installing Diva Wifi and registering the birds module:
mautil.exe -reg "../addins-registry" reg-update
mautil.exe -reg "../addins-registry" rep-add http://metaverseink.com/repo
mautil.exe -reg "../addins-registry" rep-update
@rem You can now see the list of all the available addins:
mautil.exe -reg "../addins-registry" list-av
@rem Install the Diva.Wifi addin:
mautil.exe -reg "../addins-registry" install Diva.Wifi
mautil.exe -reg "../addins-registry" install Diva.MISearchModules
