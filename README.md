### ABOUT:

2k2mv is a small tool to convert some of the assets and data from RPG Maker 2000 games to RPG Maker MV.

It is meant to be used together with [LCF2XML](https://easyrpg.org/tools).

Currently it can convert:

- Maps and map tree (without events);
- Tilesets and tileset images;
- Parallax backgrounds.


Convertion is not perfect (mainly because of different processing of autotiles in RM2000 and RMMV), but, depending on the map complexity, it can get about 90-95 % accurate.

Maps using only normal tiles and conventional "narrow border" static autotiles can be converted 100 % accurate.



### Requirements:

- Microsoft .NET Framework 4.6.1 or later.
- [LCF2XML](https://easyrpg.org/tools) from EasyRPG Tools.



### Usage:

- Convert *.ldb, *.lmt and *.lmu files in the game directory with LCF2XML;

- Set input directory to RM2000 game directory, and output directory to empty directory or to a copy of RMMV project directory;

- Load map names list and/or autotile substitution list if necessary;

- Check needed checkboxes;

- Click the convert buttons.

 
 
### IMPORTANT:

Before using this tool, the following game files must be converted with LCF2XML:
- RPG_RT.ldb 	(Database)
- RPG_RT.lmt 	(Map tree)
- Map*.lmu 	(maps)


For easy converting, put lcf2xml.exe and lcf2xml.bat (included with this tool) files in the game directory, and run lcf2xml.bat. It will automatically convert all necessary files.


### Known problems:
- Only conventional autotiles with border width less then half tile size (8 px) can be correctly converted. Other autotiles (with wide border or comprising of normal tiles) must be edited beforehand, or replaced manually with normal tiles from D tilesheet in editor, or added to autotile substitution list to be replaced automatically.

- Edges between animated autotiles can be partly non-smoothed if in close proximity to another tiles (for example, edge between shallow and deep water in close proximity to coast).



### Input Directory:

Directory with RPG Maker 2000 game. Must contain:  
- /ChipSet directory for tileset images converting.
- /Panorama directory for parallax backgrounds converting.
- RPG_RT.edb file for tileset data converting.
- RPG_RT.emt and Map*.emu files for maps converting.


### Output Directory:

Directory where converted files will be outputted.

Can be set to empty directory or RPG Maker MV project directory. In the latter case it is better to use copy of main project directory, because converting features are highly experimental and can potentially break already existing maps.



### Convert Tileset Data:

Converts tileset data from RM2000 database and writes it to /data/Tilesets.json in the output directory.

This feature is meant to use together with "Convert Tileset Images" feature.

If /data/Tilesets.json in the output directory already exists and has more tileset records then RM2000 database, it will be updated instead of overwritten.

For example, if RM2000 database has 300 tilesets, and existing Tilesets.json has 320 tilesets, then first 300 entries will be replaced with RM2000 tilesets, and last 20 will be left intact.

If some tilesets use images, not present in /ChipSet directory (usually images from RTP), the list of such images will be written to "Missing Chipset Images.txt" file in the output directory.

	
	
#### "Also copy missing chipset images from RTP to the input directory" checkbox:

If checked, the programs will copy RTP chipset images (only actually used in database) to /ChipSet directory in the input directory, so they can next be converted with "Convert Tileset Images" feature.


It tries to detect RTP location in registry, if unsuccesful, then rtpPath from 2k2mv.ini will be used (can be set manually).  
WARNING: RTP assets can only be used when owning a license for RPG Maker 2000.



### Convert Tileset Images:

Converts chipset images from /ChipSet directory in the input directory to /img/tilesets directory in the output directory.


This feature is standalone and can be used without "Convert Tileset Data" feature and LCF2XML.


Each RM2000 chipset image converts to five RMMV tileset images:
- A1 	- 	animated autotiles;
- A2 	- 	static autotiles;
- B 	- 	lower layer normal tiles;
- C 	- 	upper layer tiles;
- D 	- 	copy of static autotiles treated as normal tiles (not necessary for maps conversion, but useful for manual fixing defects in converted maps caused by unconventional use of RM2000 autotiles; also used by "Autotile Substitution List" feature).



### Convert Maps:

Converts the map tree and maps, and writes them to /data/MapInfos.json and /data/Map*.json files in the output directory.


Also converts parallax backgrounds from /Panorama directory in the input directory to /img/parallaxes directory in the output directory.


For maps to display correctly, tileset images and data must be converted first.


If Map Names List is specified, it will be used for map names in the map tree and map display names in the maps.


If Autotile Substitution List is specified, it will be used to substitute specific subtiles of listed autotiles with their normal equivalents from D tilesheet.


The structure of Map Names List and Autotile Substitution List .json files is detailed in the end of this Readme.



### "Update only tile data in existing maps and do not overwrite MapInfos.json" Checkbox:

If checked, it will update only tile data in the already existing maps in the output directory, leaving all other data(like events) intact.


WARNING: map sizes in RM2000 and RMMV must be the same.


Also, if map tree (/data/MapInfos.json) already exists in the output directory, it will not be overwritten.


### Map Names List:


JSON file, where each element contains map id, name and display name to be used during map conversion.

Map Names List file sample structure:
```
[
{
	"id":1,
	"name":"0001 First Map",
	"displayName":"First Map"
},
{
	"id":2,
	"name":"0002 Second Map",
	"displayName":"Second Map"
}
]
```


### Autotile Substitution List:

JSON file, where each element contains tileset id and list of zero-based indexes (from 0 to 11) of problematic autotiles in the tileset.


These autotiles will have specific subtiles substituted with their normal equivalents from D tilesheet during map convertion.


Autotile Substitution List file sample structure:
```
[
{
	"id": "5",
	"tileIndex": [0,9,11]
},
{
	"id": "10",
	"tileIndex": [4]
}
]
```