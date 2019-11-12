use strict;
use warnings;
use 5.010;

use File::Copy;
use File::Path;

my $v = "3.26";

my $type  = '-V' . $v; 
use Cwd;
my $dir = getcwd;


say ('Server Publish? <enter for no>');
my $publish = <stdin>;
chomp $publish;


if ($publish)
{
	say ("Unlinking");	
	unlink "y:/Inetpub/Secondlife/Outworldz_Installer/Grid/DreamGrid.zip" || die ;
	unlink "y:/Inetpub/Secondlife/Outworldz_Installer/Grid/DreamGrid-Update.zip" || die ;
	unlink "y:/Inetpub/Secondlife/Outworldz_Installer/Grid/Older Versions/DreamGrid-Update$type.zip" || die $!;
	unlink "y:/Inetpub/Secondlife/Outworldz_Installer/Grid/Older Versions/DreamGrid$type.zip" || die $!;
}


say("Clean up opensim");
my @deletions = (
	"$dir/OutworldzFiles/AutoBackup",	
	"$dir/OutworldzFiles/Opensim/WifiPages-Custom",	
	"$dir/OutworldzFiles/Opensim/bin/WifiPages-Custom",
	"$dir/OutworldzFiles/Opensim/WifiPages",	
	"$dir/OutworldzFiles/Opensim/bin/WifiPages",
	"$dir/OutworldzFiles/Opensim/bin/datasnapshot",
	"$dir/OutworldzFiles/Opensim/bin/assetcache",
	"$dir/OutworldzFiles/Opensim/bin/j2kDecodeCache",
	"$dir/OutworldzFiles/Opensim/bin/MeshCache",
	"$dir/OutworldzFiles/Opensim/bin/ScriptEngines",
	"$dir/OutworldzFiles/Opensim/bin/maptiles",
	"$dir/OutworldzFiles/Opensim/bin/Regions",
	"$dir/OutworldzFiles/Opensim/bin/bakes",
	"$dir/OutworldzFiles/Opensim/bin/addin-db-002",
	"$dir/OutworldzFiles/Opensim/bin/fsassets",	
	"$dir/OutworldzFiles/Apache/logs/",
);

foreach my $path ( @deletions) {
	say ($path);
	DeleteandKeep($path);
}

DelZips();
DelMaps();

unlink "$dir/BareTail.udm";
unlink "$dir/DreamGrid.zip";
unlink "$dir/OutworldzFiles/Apache/htdocs/Search/flog.log" ;
unlink "$dir/OutworldzFiles/PHP5/flog.log" ;
unlink "$dir/OutworldzFiles/Opensim/bin/Error.log" ;
unlink "$dir/OutworldzFiles/Opensim/bin/Opensim.log" ;
unlink "$dir/OutworldzFiles/Opensim/bin/Opensimstats.log" ;
unlink "$dir/OutworldzFiles/PHPLog.log" ;
unlink "$dir/SET_externalIP-log.txt";
unlink "$dir/OutworldzFiles/Photo.png";
unlink "$dir/OutworldzFiles/XYSettings.ini";
unlink "$dir/Outworldzfiles/Icecast/log/error.log" ;
unlink "$dir/Outworldzfiles/Icecast/log/access.log" ;
unlink "$dir/UpdateGrid.log";
unlink "$dir/OutworldzFiles/Opensim/bin/OpensimConsoleHistory.txt" ;
unlink "$dir/OutworldzFiles/Opensim/bin/LocalUserStatistics.db" ;

#Setting
unlink "$dir/Outworldzfiles/Settings.ini" ;


#logs
unlink "$dir/OutworldzFiles/Diagnostics.log" ;
unlink "$dir/OutworldzFiles/Outworldz.log" ;
unlink "$dir/OutworldzFiles/Init.txt" ;
unlink "$dir/OutworldzFiles/upnp.log" ;
unlink "$dir/OutworldzFiles/http.log" ;
unlink "$dir/OutworldzFiles/Error.log" ;
unlink "../Zips/DreamGrid$type.zip" ;
unlink "../Zips/Outworldz-Update$type.zip" ;

DeleteandKeep("$dir/OutworldzFiles/AutoBackup/MySQL");


say "DLL List Build";
use File::Find;

open (OUT, ">", 'dlls.txt');

find({ wanted => \&process_file, no_chdir => 1 }, $dir . '/Outworldzfiles/opensim/bin/');

sub process_file {
   # if (-f $_) {
       # print "This is a file: $_\n";
    #} else {
       # print "This is not file: $_\n";
   # }
	if ($_ !~ /dll$/) {
		return;
	}
	my $fullpath = $_;
	$fullpath =~ s/$dir//g;
	$fullpath =~ s/\//\\/g;
	
	print OUT $fullpath . "\n";	
}

close OUT;

my $exes = "$dir/Installer_Src/Setup DreamWorld/bin/Release";
sign($exes);

use File::Copy::Recursive qw(dircopy);
dircopy($exes,$dir) or die("$!\n");

unlink "$dir/Start.exe.lastcodeanalysissucceeded";
unlink "$dir/Start.exe.CodeAnalysisLog.xml";


say("Signing");
use IO::All;
sign($dir);


say("Mysql");
chdir(qq!$dir/OutworldzFiles/mysql/bin/!);
print `mysqladmin.exe --port 3309 -u root shutdown`;
print `mysqladmin.exe --port 3306 -u root shutdown`;

sleep(5);
chdir ($dir);
DeleteandKeep("$dir/OutworldzFiles/mysql/data");


print "Processing Main Zip\n";

JustDelete('O:\\Opensim\\Zip');

my @files =   `cmd /c dir /b `;

foreach my $file (@files) {
	chomp $file;
	next if -d "$dir/$file";
	next if $file =~ /^\./;
	ProcessFile ("\"$dir\\$file\"" );
}

say("Adding folders");


ProcessDir ('MSFT_Runtimes');
ProcessDir ('Licenses_to_Content');
ProcessDir ('OutworldzFiles\\Apache');
ProcessDir ("OutworldzFiles\\AutoBackup");
ProcessDir ("OutworldzFiles\\Help");
ProcessDir ("OutworldzFiles\\IAR");
ProcessDir ("OutworldzFiles\\Icecast");
ProcessDir ("OutworldzFiles\\Mysql");
ProcessDir ("OutworldzFiles\\OAR");
ProcessDir ("OutworldzFiles\\PHP7");
ProcessDir ("OutworldzFiles\\Opensim");
ProcessDir ("OutworldzFiles\\fr-FR");
ProcessDir ("OutworldzFiles\\en");



say("Drop mysql files from update");
# now delete the mysql from the UPDATE

DeleteandKeep('O:\\Opensim\\Zip\\Outworldzfiles\\mysql\\Data');
say("Drop Opensim Source code from update");
JustDelete('O:/Opensim/Zip/Outworldzfiles/Opensim/Opensim');
JustDelete('O:/Opensim/Zip/Outworldzfiles/Opensim/addon-modules');
JustDelete('O:/Opensim/Zip/Outworldzfiles/Opensim/Doc');
JustDelete('O:/Opensim/Zip/Outworldzfiles/Opensim/Prebuild');
JustDelete('O:/Opensim/Zip/Outworldzfiles/Opensim/share');
JustDelete('O:/Opensim/Zip/Outworldzfiles/Opensim/Thirdparty');
JustDelete('O:/Opensim/Zip/Outworldzfiles/Opensim/.git');
JustDelete('O:/Opensim/Zip/Outworldzfiles/Opensim/.gitignore');
JustDelete('O:/Opensim/Zip/Outworldzfiles/Opensim/.hgignore');
JustDelete('O:/Opensim/Zip/Outworldzfiles/Opensim/BUILDING.md');
JustDelete('O:/Opensim/Zip/Outworldzfiles/Opensim/compile.bat');	
JustDelete('O:/Opensim/Zip/Outworldzfiles/Opensim/makefile');
JustDelete('O:/Opensim/Zip/Outworldzfiles/Opensim/nant-color');
JustDelete('O:/Opensim/Zip/Outworldzfiles/Opensim/BUILDING.md');	
JustDelete('O:/Opensim/Zip/Outworldzfiles/Opensim/Opensim.sln');
JustDelete('O:/Opensim/Zip/Outworldzfiles/Opensim/Opensim.build');
JustDelete('O:/Opensim/Zip/Outworldzfiles/Opensim/prebuild.xml');
JustDelete('O:/Opensim/Zip/Outworldzfiles/Opensim/runprebuild.bat');	
JustDelete('O:/Opensim/Zip/Outworldzfiles/Opensim/runprebuild.sh');
JustDelete('O:/Opensim/Zip/Outworldzfiles/Opensim/TESTING.txt');
JustDelete('O:/Opensim/Zip/Make_zip_v3.pl');
JustDelete('O:/Opensim/Zip/Make_zip_v2.pl');


#####################
print "Make zip\n";
unlink "O:/Opensim/Zips/DreamGrid$type.zip";
my $x = `../7z.exe -tzip -r a  O:\\Opensim\\Zips\\DreamGrid$type.zip O:\\Opensim\\Zip\\*.*`;

sleep(1);

if ($publish)
{
	say ("Publishing now");
	
	if (!copy ("../Zips/DreamGrid$type.zip", "y:/Inetpub/Secondlife/Outworldz_Installer/Grid/DreamGrid.zip"))  {die $!;}
	if (!copy ("../Zips/DreamGrid$type.zip", "y:/Inetpub/Secondlife/Outworldz_Installer/Grid/DreamGrid-Update.zip"))  {die $!;}
	if (!copy ("../Zips/DreamGrid$type.zip", "y:/Inetpub/Secondlife/Outworldz_Installer/Grid/Older Versions/DreamGrid$type.zip"))  {die $!;}
	if (!copy ("../Zips/DreamGrid$type.zip", "y:/Inetpub/Secondlife/Outworldz_Installer/Grid/Older Versions/DreamGrid-Update$type.zip"))  {die $!;}
	if (!copy ("../Zips/DreamGrid$type.zip", "E:/Dropbox/Dreamworld/Zip/DreamGrid.zip"))  {die $!;}
	
	print "Revisions\n";
	if (!copy ('outworldzfiles\\Help\\Revisions.rtf', 	'y:/Inetpub/Secondlife/Outworldz_Installer/Grid/Revisions.rtf'))  {die $!;}
	if (!copy ('Revisions.txt', 						'y:/Inetpub/Secondlife/Outworldz_Installer/Grid/Revisions.txt'))  {die $!;}
	if (!copy ('Revisions.txt', 						'y:/Inetpub/Secondlife/Outworldz_Installer/Revisions.txt'))  {die $!;}

}



say "Done!";

sub Write
{
	my $dest = shift;
	my $content = shift;
	open OUT, ">$dest" || die $!;
	print OUT $content;
	close OUT;
}

sub ProcessFile
{
	my $file = shift;
	
	my $x = `xcopy $file ..\\Zip\\`;
	$x =~ s/\n//g;
	if ($x =~ /File\(s\) copied/) {
		print "$file ok\n";
	} else {
		print "$file Fail: $x\n";
		exit;
	}
	
	
}

sub ProcessDir
{
	my $file = shift;
	
	my $x = `xcopy /E /I O:\\Opensim\\Outworldz_Dreamgrid\\$file O:\\Opensim\\zip\\$file`;
	$x =~ s/\n//g;
	if ($x =~ /File\(s\) copied/) {
		print "$file ok\n";
	} else {
		print "$file Fail: $x\n";
		exit;
	}
	
	
}

sub rm {
	
my $path = shift;
	
	my $errors;
	while ($_ = glob("'$path/*'")) {
		rmtree($_)
		  or ++$errors, warn("Can't remove $_: $!");
	}
	
	#exit(1) if $errors;
}

sub JustDelete {
	
	my $path = shift;
	if (! -d $path) {
		unlink $path;
	}
		
	use File::Path;	
	rmtree $path;	
}

sub DeleteandKeep {

	my $path = shift;	
	use File::Path;	
	rmtree $path;	 
	while (-e $path) 
    {
		rmtree $path;	 
        print "Directory '$path' still exists\n";
		sleep(1);
    }
    
	mkdir $path ;
	open (FILE, '>', $path . '/.keep') or die;
	print FILE "git will not save empty folders unless there is a file in it. This is a placeholder only\n";
	close FILE;
	
}


# for importers
sub Perlunzip {
	
	use Archive::Zip qw(:ERROR_CODES :CONSTANTS);
	use Exporter 'import';
	
	my ($zip_file , $out_file, $filter) = @_;
	$zip_file = $dir . '/Outworldzfiles/'. $zip_file;
	$out_file = $dir . '/Outworldzfiles/'. $out_file;
	
	my $zip = Archive::Zip->new($zip_file);
	unless ($zip->extractTree($filter || '', $out_file) == AZ_OK) {
		warn "unzip not successful: $!\n";
	}
}


sub DelZips
{
	while ($_ = glob("$dir/*.zip")) {
		unlink ($_)  or die("Can't remove $_: $!");
	}	
}


sub DelMaps
{
	while ($_ = glob("$dir/Outworldzfiles/opensim/bin/Map-*.png")) {
		unlink ($_)  or die("Can't remove $_: $!");
	}	
}

sub sign
{
		
	my @files = io->dir(shift)->all(0);  

	my @signs;
	foreach my $file (@files) {
		my $name = $file->name;
		next if $name =~ /Microsoft|Debug|\.git|baretail|Downloader|Bouncy|Google|Tuple/;
		if ($name =~ /dll$|exe$/ ) {
			
			my $r = qq!../Certs/sigcheck64.exe "$name"!;
			print $r. "\n";
			my $result1 = `$r`;
			if ($result1 =~ /Publisher:.*Outworldz, LLC/) {
				next;
			}
			$result1 =~ s/\n//g;
			if ($result1 =~ /Verified(.*)/i) {
				push(@signs,$name);
			};
			
			
			my $f = qq!../Certs/DigiCertUtil.exe sign /noInput /sha1 "D7EA8E5F8E6D27B138ECD93811DAA6B02B0BA333" "$name"!;
			print $f;
			my $result = `$f`;
			print $result. "\n";
			$result =~ s/\n//g;
			if ($result =~ /Verified(.*)/)
			{
				say($1);
			}
			if ($result !~ /success/) {
				say ("***** Failed to sign!");
				die;
			}
		}
	}
	
	say (join("\n",@signs));
}