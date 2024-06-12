This folder structure is subject to change, but in the meantime here are the required content files to run various optional parts of ACRealms

If UseRealmSelector is enabled in Config.realms.js, you must manually copy the contents of "Content/extensions/Realm Selector" to the root of your content folder.
After copying, your content folder shou

Eventually there will be a more convenient way to do this (such as enabling extensions by listing the names of them in a config, and all the content imports will be handled automatically)

FYI: Weenies can support subfolders, but landblocks and realms do not at this point 
(I plan to fix that, and was the first to add support in ACE for importing from all weenie subfolders at once)

