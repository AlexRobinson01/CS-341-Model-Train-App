# CS-341-Project

Proposal:

- Our idea for an app stems off the idea for a model train track designer app, where the user would be able to design their own track using a set of pieces they choose from. They can save and load their own designs, or load someone else’s design if they have the key for it. Track limits can be set for designs if someone wants to only use the pieces they have, and they can even take a picture of where they want to put a track to see how it’ll look! Building a track would allow a user to either drag and drop pieces that snap together, or they can draw a line from a previous segment and let the app decide what angle they were trying to use.
- This app would be most useful for model train hobbyists, as it lets them bring their track ideas to life, without spending so much time assembling and disassembling physical tracks if a design doesn’t work! It’s cool to be able to lay out your designs and know they’ll work without needing to assemble everything by hand first, and it’s essential for saving time on these projects!

We plan to communicate via Discord, and meet in-person after each class (Tuesday and Thursday, around 4:30 until whenever the meeting concludes).

Existing account details: userid - "admin", pwd - "1234"

Sprint 4 promised tasks (with a checkmark by those that are completed):

- Implement personalized account screen so account info reflects user ✅
- Populate personal projects from database for specific user ✅
- Test all functionality ✅
- Write comments ✅
- Add delete project functionality for PersonalProjects ✅
- Use bcrypt to add more secure authentication
- Implement rendering track pieces to the editor canvas ✅
- Implement snapping track pieces together by position ✅
- Add functionality for the Piece Catalog screen ✅
- Add functionality for the Piece Editor screen
- Finish functionality for dragging pieces around the editor canvas ✅
- Finish creating images for basic track pieces
- Implement saving/loading of a local settings file with user preferences ✅
- Add consistent autosave functionality to the track editor (async) ✅
- Implement camera functionality for track editor background

Sprint 3 Changes
- Implemented credential validation for login/create account screens :)
- Created drag/drop functionality for pieces on the track editor (currently not rendered, will be finished next sprint following the addition of piece images) :) :)
- Hotbar items now appear in the track editor
- Created the API for a local database for the user's settings and hotbar layout
- Fixed some portrait/landscape and navigation bugs
- Created some images for track pieces :)
- Adjusted Collaborative Storage appearance and added home buttons to various screens

Sprint 2 Changes
- Added database connectivity to populate account screens profile circle, name, and email from database :)
- Added business logic and database layers to run database methods through
- Added more file organization with a model folder and various categorical folders
- Added navigation between all screens :)
