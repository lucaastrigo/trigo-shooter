Thankyou for purchasing **Fancy Folders** by Placeholder Software!

Getting Started
===============

To get started with customising files and file just press ALT and click on a file or folder in the project browser. This will pop up an editor window which allows you to customise that item.

Files
=====

Files can be customised to add detail text to them - this only shows up in "One Column Layout" mode. To change the layout mode  right click the "Project" tab and select "One Column Layout".

The detail text can include metadata which will be automatically updated as the file changes:
* {name}   - Name of the file
* {size}   - Size of the file
* {ext}    - File extension
* {guid}   - Unity unique ID for this file
* {status} - Git status of this file (e.g. Added/Modified/Conflicted)

To discard changes and close the editor window click "Cancel". To discard changes click "Revert". To change to the default settings for this file click "Default". To apply the changes click "Apply".

Folders
=======

Folders can be customised to add detail text to them - this shows up in all layout modes. The detail text can include metadata which will be automatically updated as the contents of the folder change:
* {name}   - Name of the folder
* {size}   - Size of all the items in the folder and subfolders
* {count}  - Count of all the items in the folder and subfolders
* {guid}   - Unity unique ID for this folder
* {status} - Most important status of any item in this folder or subfolder

The appearance of folders can also be customised. Change the "Background Icon color" to change the color of the folder. Choose an "Overlay Icon" to draw an icon on top of the folder. Change the "Foreground Icon color" to change the color of the overlaid icon.

To apply the settings to this folder and all subfolders check the "Recursive" checkbox.

To discard changes and close the editor window click "Cancel". To discard changes click "Revert". To change to the default settings for this folder click "Default". To apply the changes click "Apply". 

Global Settings
===============

Global settings can be modified by inspecting the "Fancy Folders Settings" file located in "Assets/Plugins/Fancy Folders".

* Icon Set - Configure the icon set in use (see the icon set section below).
* Show Status Icon - Toggle whether automatic status icons are shown.
* Icon Column Count - Set how many file type mini icons are displayed next to each folder.

* Default Directory Format - Set the default detail string to apply to folders which have no detail string set.
* Default File Formay - Set the default detail string to apply to files which have no detail string set.

* Use Custom Text Color - If checked, you can choose the color of the detail text.
** Custom Text Color - Choose the color of detail text (only visible if "Use Custom Text Color" is selected).

* Show Line Separators - Toggle whether lines will be drawn between items.
* Use Custom Separator Color - If checked, you can choose the color of separators.
** Separator Color - Choose the color of separators (only visible if "Use Custom Separator Color" is selected).

* Show Selection Outline - Toggle whether a box will be drawn around selected items.
* Use Custom Selection Outline Color - If checked, you can choose the color of the selection box.
** Select Outline Color - Choose the color of selection boxes (only visible if "Use Custom Selection Outline Color" is selected).

* Search - Filter the list of settings by the search term
** List - A list of all settings for files and folders in this project. Click the "X" to delete a setting (reverting that item to default settings).

Icon Set
========

The icon set can be modified by inspecting the "Default Icon Set" file located in "Assets/Plugins/Fancy Folders". The icon set controls which textures are used to draw specific icons. A custom icon set can be used by copying the "Default Icon Set" file and referencing the new file in the "Global Settings - Icon Set" field.

* Large Overlay Icons

These are the icons which can be overlaid on folders. Each icon can have 4 textures associated with it: Large and Small in High and Low DPI variants. The small version is used for small icons in the left column of "Two Column Layout" mode or in the "One Column Layout" mode, The large version is used for the large icons on the right of the "Two Column Layout" mode. The DPI version is automatically selected based on the pixel density of the screen it is being displayed on. If some variants are not configured it will automatically fall back to using the next best variant.

* Custom Icons

Custom icons can be added to a list of named icons. Each icon in this list has the same 4 textures as other large icons as well as a name. To add a custom icon:
 1. Increase the list count in the inspector
 2. Give the new icon a unique name
 3. Choose texture(s) for the icon
 4. Alt+Click on a directory, set "Overlay Icon" to "Custom"
 5. Set the "Custom Icon ID" to the name of the icon you just created

* Mini Icons

These are the icons drawn into the stack of icons by a folder, used to indicate file type and status. Each icon has two textures associated with it: High and Low DPI. The DPI version is automatically selected based on the pixel density of the screen it is being displayed on.

Further Support
===============

If something went wrong, these docs are confusing, you want to request a feature or give us some other feedback:

To report bugs or request features, please open an issue on the issue tracker:

 > https://github.com/Placeholder-Software/FancyFolders
 
If you're really stuck, send us an email:

 > admin@placeholder-software.co.uk
 
If you think **Fancy Folders** is great, give us a review and tell everyone why:

 > https://assetstore.unity.com/packages/slug/143763?aid=1100lJDF









