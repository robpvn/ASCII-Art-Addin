# ASCII ART Import/Export plugin

This plugin is based on the Paint.Net plugin "Another ASCII Art File Type Plugin"
by the forum user "I Like Pi", licensed under the GPLv3.

The original is available at http://forums.getpaint.net/index.php?/topic/8646-another-ascii-art-file-type-plugin/
([Archive.org backup link](https://web.archive.org/web/20120926100408/http://forums.getpaint.net/index.php?/topic/8646-another-ascii-art-file-type-plugin/))

This has been done in an evening's work, in order to prove that it can be simple 
and easy to port existing Paint.Net plugins (at least from the era around when Pinta was started).

The original code has been dissasembled into multiple files for more tidiness,
and the actual plugin registration is different in Pinta, but the non-gui
code is pretty much identical. (Thanks to Pinta's Cairo Extensions!) The original
code is available in the Original_code folder.
