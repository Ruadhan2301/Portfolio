This project has been built as a demonstration of Vue.JS and Firebase

It comprises a login/authentication/registration system hooked to a Firebase account.

You can create a new account (which will automatically log you in)
You can log out of an account, and you can log back in.

You can do this any number of times, the accounts are persistent between sessions because they are hosted on a live firebase database/auth system.

There is some validation on the accounts, you must for example always include a name, valid regex'd email and a password of sufficient complexity.

The primary operating script is 
Scripts/vue-components/navbar.js
It was going to be multiple files, but in the end there wasn't enough to justify splitting it up.

For a larger project I would have done so as there'd be more components to manage!

There are notes in the code to explain some of the more interesting/complex aspects of it, most of it is out-of-the-box Firebase code or fairly standard Vue.JS