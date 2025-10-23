# How to use:

## 1. Disable immediate file removal in windows
**Go to recycle bin settings and make sure the setting 
"Don't move files to Recycle Bin. Remove files immediatelly when deleted."
is DISABLED.**

**I OFFER NO FILE RECOVERY ASSISTANCE IF YOU DELETE SOMETHING IMPORTANT ON ACCIDENT.
YOU HAVE BEEN WARNED.**


## 2. Installation
The program is a standalone executable. Unpack/download the program into folder of your choosing and run the exe. You can put it in program files and add a shortcut if you wish. All program settings and cache will be stored in program folder. Does not touch registry.


## 3. Basic configuration:
Click on Options->Config panel.
The default configuration is to ignore empty files, all other options are disabled.

Available options and what they do:
- "Use CRC32" - This is an older algorithm for calculating file hashes. It may be faster on very old machines that do not support MD5 in hardware. If unchecked, program will use MD5.
- "Ignore empty files" - Files with size = 0 will be ignored. You will be surprised how many empty files one can have, and they will all egister as duplicates, because empty equals empty.
- "Count folder files" - (Warning - negatively affects performance!) This will add counter showing how many files the folder contains, in addition to how many duplicate files are in the folder. Useful if you are trying to decide between "All photos 2025" and "That trip to Rome" folders. Affects performance because it needs to read contents of every folder.
- "Use hash caching" - (WARNING: Privacy risk!)(WARNING: Stale data risk) Enable to cache the file checksums. This greatly speeds up repeated scans, but has privacy and data integrity risks. 
	Privacy risk: The cache file will store a standarized checksum of all files that have been identified as duplicates. If a hostile agent gets access to this file they can determine what uniquely identifiable files were present on your computer at some time. If you store/process files that you do not want anyone else to know about, do not enable this option. You can delete the cache with "Delete cache" button or manually remove the "cache.tsv" from program folder.
	Stale data risk: As of v2025.04.12, the program does not rescan files that were cached. If you modify a file without changing its size or location, program will use the old stale cached checksum, which may lead to deleting file that is different from original. Use this option for scanning files that you know will never change (eg photo collection, music, movies). Do not use this for save games, especially ones coming in fixed size slots.
- "Size" boxes. Input minimum and maximum size of files to scan, in kilobytes. Do note that windows shows files smaller than 1024 bytes as 1kb. Program will use the true size. Leave the first box blank if you want to scan files smaller than 1kb.
- "Preview enabled" - (Warning - negatively affects performance) Enables thumbnails for duplicate list. This can be handy if you need to see what the file contains before deciding which folder it should go into. Experimental, may slow down program significantly or even crash it.


## 4. Main program window - how to use.
Begin by clicking "Browse" button to select folder to scan. If you wish to scan multiple folders, click the "+" button next to browse button. 
You can add ignore pattern by typing into the "ignore box". Simple wildcards supported, divide options with commas. Example "thumbs.db" to skip generated thumbnails files.
Click start and wait. The scan may take a while - observe the bottom progress bar. It will fill up once for listing the files in folders, then once again when program starts comparing them.
You can pause or cancel the scan at any time. 
If you enabled the cache option, the program will reuse the already calculated checksums on next scan - otherwise, it has to do all the work again.
You can however cancel the scan midway and delete the file duplicates already found - if you don't mind working on partial results.


## 5. Result list.
Result list shows files that are identical grouped into sets. Each line shows icon for file type (or thumbnail if preview is enabled), followed by full file path, number of duplicates in the folder, total number of files in the folder if enabled, and file size in bytes.
- You can click on individual files to open them. Do mind this will open them just like in explorer, so PLEASE be careful with executable files.
- Duplicate count column shows how many duplicate sets are in the folder where the file is located. If it says 4, that means four of the files have a duplicate somewhere in another folder. This will be useful when bulk marking below.
- You can sort the columns by clicking headers. First column will sort by filepath (useful if you want to process duplicates from given drive first).
- You can mark a file and press "delete" button on keyboard to delete marked file.
- You can select multiple files using ctrl or shift keys and delete them this way as well. Do take care that if you select all the entries in a group, you will be deleting ALL copies of the file from your drive. Do this only when you are ceratin all the copies are junk.
- You can right-click any of the files for following options:
	- "Open containing folder" - opens new explorer window with the folder the file is located in. You can asses if this is the folder you want to keep the file in. If you do any manual changes to files - deletion, renames, moving folders, please rerun the scan so the program can update the list.
	- "Add to trashlist" - this will mark the whole folder as trash - all duplicate files in this folder will be deleted once "Mark trash" and "Delete marked" buttons are pressed. Do note that selection is recursive (all the subfolders are affected). If all copies of the duplicate reside in the Trash folder, they will not be touched. Program never deletes the last copy (unless you do it manually).
	- "Add to keeplist" - this option is reverse of the above - all files in this folder will be kept and program will delete any duplicate copies found outside the folder.
Once you added folders to Trash/Keep list, press "Mark Trash" button to preview files that will be deleted. The list will mark them in ed and sort the files onto top of the list. Use "Delete Marked" button to remove the marked items.
You can add folder manually to Keep/Trash list by using the buttons under the lists. Or remove them, clear them and sort them.


## 6. Additional functions:
- "Mark extra copies within folder" - found in "Actions" menu, this function will find folders that contain multiple copies of same file and mark all but last one for deletion. Press "Delete marked" to remove the files. Useful if you have multiple copies of "cake recipe.pdf" in your downloads folder.


## x. How does the file comparing works?
Program compares the file size (different sizes = different files), then calculates the checksum of the file. If checksums match, the files are considered identical.
There is some speedup magic included for larger files - program will compare small chunk from middle of the files to determine if they have the same contents, and only if the middle is identical, calculate full checksum.




# Change history:

v0.9 - Splitted project into UI & Engine.

v0.9b - Fixed bug with access rights. Attempting to list files/folders that user has no rights to will no longer crash the app.

v0.9c - Fixed bug with "funny" directory names parsing incorrectly. Fixed issue with non-accessible files crashing.

v0.9d - Removed external lib and replaced it with old homemade crc32.

v0.9h - Architectural changes. 
		Duplicate list is now cleared at start. Minor fix to prevent result multiplication when refreshing partial result. Start will now clear deletion list. 
		Fixed bug when keep/trash zone folder name was a substring of another folder
		Changed how keep/trash zone works:
		 - keepzone will keep all the files inside, even if duplicated, and delete every dupe outside.
		 - trashzone will remove all the dupes if there is at least one copy outside trashzone. Otherwise, it keeps all the copies in trashzone.

v0.9i - Bugfixing:
		Status does not say "comparing" after listing files - fixed

v0.10 - Added ability to skip empty files.
        Fixed issue with list not clearing when new scan is started.

v0.11 - Fixed issue with Trash/Keep list selection overriding the root Browse folder.

v0.12 - Fix crash when extracting icon for deleted file.
        Added duplicate Count column with sorting.

v0.13 - Added saving of settings to registry.
        Added filtering by size.
		Fixed issue with path being cleared when browse dialog is exited by cancel.

v0.14 - Performance improvements

v0.15 - Feature: Show total folder file count next to duplicate count. Can be enabled in config panel/
		Config panel now shows the program version.

v2017.11.09 - Improvement: On PathTooLong offer opening the location of offending folder.

v2018.03.13 - Added functionality to mark for deletion extra copies within same folder. Use from Actions menu.

v2018.04.08 - Feature: cache the hash results. Checksums for files that have been hashed already will be cached in a file now. 
			  This will speed any further re-runs of program significantly. To clear the cache, simply delete the cache.tsv from program folder. 
			  Program will re-hash files that changed location, but not the ones where content has changed. Use for files where content does not change e.g. images.
			  Feature can be turned on in config pane.

v2018.04.10 - Bugfixes: Crash on malformed cache file (will ignore invalid lines now).
			            Renamed cache.csv to cache.tsv, since it's a tab separated values, not comma.
						Change cache writeout to be every 1GB data processed instead of every 100 files.

v2018.04.15 - Optimization refactor. UI is now using virtual mode for results list. This release is considered UNSTABLE.

v2018.04.15 v2 - Fixed flicker on list redraw, by improving the performance of item calculation. The items will still flicker somehow on mousemove, 
                 because WindowsForms insist on redrawing whatever the mouse is moving over. May get fixed if annoying enough.
				 Readme (this file) is now included in program folder by default, yay!

v2018.04.19 - Performance improvements. Should have smaller memory footprint now.

v2018.05.09 - Cache save when indexing finishes. View will now unmark selected item and scroll to top when using "Mark Trash" and "Delete Marked" buttons.

v2018.08.05 - Renamed find trash to mark trash. Added 'clear' buttons to trash/keep list. 
			  Note: Mark trash will encounter performance issues when lists are long - use clear to wipe them periodically. This is a workaround until better solution is implemented.
			  Fixed: Exception when last item is delete with Delete marked button.
			  Changed default sort order to descending. Delete Marked will now restore previous sort column after deleting the files.
			  Selected index staying the same after deletion and marking another file is now considered an feature, not bug.

v2020.02.04 - Some backend cleanup, perf improvements, hopefully fixed index out of range when scanning.
		      UI is now disabled when listing files/comparing.
			  Added progress bar on marking and deletion operations.

v2022.09.01 - Added preview mode.

v2023.04.20 - Added option to delete cache file. Added warning when scan folder does not exist.

v2023.10.28 - Added option to scan multiple folders at once.

v2023.10.29 - Added option to sort by folder total file count.
			  Minor bugfixes.

v2023.11.04 - Bugfix: Avoid file readonly dialog when deleting.

v2023.11.17	- Added option to trim cache. Bugfix: skip recycle bin. Bugfix: make cache report progress. Bugfix: cache trimming now leaves UI intercative and can be cancelled.

v2023.11.18	- Bugfix: Make sure the cache is actually written out to disk after trimming. Added option to sort items in selection boxes.

v2023.11.20 - Bugfix: UI. Fix stray sort button. Dialogs are now centered on parent window. Fix count column duplication when closing settings panel. Hopefully improve the UI to actually be disabled when clicking "start".
			  Changed settings to be stored in file instead of registry.

v2023.11.25	- UI: Add thousand separators in status strip for readability. Bugfix: Remove cached items from cache on file delete. Improve cache file code to not crash the app.
			  Bugfix: should no longer crash when file to hash is inacessible (deleted/moved while program was running). Added option to pause the scan.

v2024.08.23 - Changed to .NET 4.7.2 (latest compatible with win 7). Windows XP is no longer supported, use v2023.11.25 or earlier.
			  Added privacy warning when enabling cache. Fixed file size limits being interpreted as bytes when they should be Kilobytes.

v2025.03.08 - Improved performance of refreshing result view - the view is now only refreshed if new items have been added during search instead of every 100ms; this should hopefully greatly decrease flickering and disk access. 

v2025.04.12 - Changed cache autosave to occur every 5 minutes instead of every 1gb of data to avoid disk trashing.

v2025.10.23 - Public release