
//Email Regex from the Firebase login tutorials, because nobody likes regex but it's gotta be done.
var emailRE = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/

const state = {
    OFF: "off",
    REGISTER: "register",
    LOGIN: "login",
    ACTIVE: "active"
}

// This is the engine for the site, everything important is happening here.
var loginSystem = new Vue({
    el: '#account-system',
    data: {
        activeUser: null,

        loginUser: {
            name: '',
            email: '',
            password: ''
        },
        newUser: {
            name: '',
            email: '',
            password: ''
        },

        state: state.OFF // Current state
    },
    computed: {
        //Vue can deal with individual variables being queried inline in the code, but it's just as messy as doing it here and harder to debug. So we do it individually.
        loginMode: function () {
            return this.state === state.LOGIN;
        },
        registerMode: function () {
            return this.state === state.REGISTER;
        },
        loggedinMode: function () {
            return this.state === state.ACTIVE;
        },
        hasUser: function () {
            return this.activeUser !== null;
        },

        displayUserName: function () {
            if (this.activeUser) {
                return "Welcome " + this.activeUser.name;
            } else {
                return ""
            }
        },

        // Essentially here we create a list of true/false states on the different variables, we can then check them individually for the error messages
        //          or collectively for the overall "isValid/isValidNewUser" functionality.
        newUserValidation: function () {
            return {
                name: !!this.newUser.name.trim(),
                password: !!this.newUser.password.trim(),
                email: emailRE.test(this.newUser.email)
            }
        },
        isValidNewUser: function () {
            var validation = this.newUserValidation
            return Object.keys(validation).every(function (key) {
                return validation[key]
            })
        },
        validation: function () {
            return {
                password: !!this.loginUser.password.trim(),
                email: emailRE.test(this.loginUser.email)
            }
        },
        isValid: function () {
            var validation = this.validation
            return Object.keys(validation).every(function (key) {
                return validation[key]
            })
        }

    },
    methods: {
        // There's no real reason to make distinct close functions for both login and register windows, so we just close both of them.
        close: function () {
            loginSystem.state = state.OFF;
        },
        login: function () {
            console.log("login button pressed");
            if (this.isValid) {//Validate that our input-data is good before we try to log in.
                // As a matter of course, I keep logic seperate from the button functionality.
                this.loginInternal(this.loginUser.email, this.loginUser.password);
            } else {
                //We could easily add a proper error for users here to tell them to finish what they're doing.
                console.log("Finish putting in the details!");
            }
        },
        register: function () {
            console.log("registering");
            if (this.isValidNewUser) {
                this.registerNewUser(this.newUser.name, this.newUser.email, this.newUser.password);
            } else {
                console.log("Finish putting in the details!");
            }
        },
        loginInternal: function (email, password) {
            initApp();
            console.log("logging in with email: " + email + " and using password: " + password);
            
            firebase.auth().signInWithEmailAndPassword(email, password).catch(function (error) {
                // Handle Errors here.
                var errorCode = error.code;
                var errorMessage = error.message;
                if (errorCode === 'auth/wrong-password') {
                    alert('Wrong password.');
                } else {
                    alert(errorMessage);
                }
                console.log(error);
            });
        },

        registerNewUser(newName, newEmail, newPassword) {
            initApp();
            console.log("creating new account for user: " + newName + " with email: " + newEmail + " and using password: " + newPassword);

            firebase.auth().createUserWithEmailAndPassword(newEmail, newPassword).catch(function (error) {
                var errorCode = error.code;
                var errorMessage = error.message;
                if (errorCode == 'auth/weak-password') {
                    alert('The password is too weak.');
                } else {
                    alert(errorMessage);
                }
                console.log(error);
            });
        },

        logout() {
            initApp();
            firebase.auth().signOut();
            console.log("user is signed out");
            loginSystem.activeUser = null;
            loginSystem.state = state.OFF;
        }
    },
    mounted: function () {
        console.log("initialised Login");
    }
});

//This block governs the Navbar functionality, specifically the three buttons on it.
// It talks directly to the loginSystem which contains the actual functionality.
var navbar = new Vue({
    el: '#top-navbar',

    computed: {
        loggedIn: function () { // Track whether we're logged in or not so the login/logout buttons display the correct state.
            return loginSystem.activeUser;
        }
    },

    methods: {
        login: function () {
            console.log("login button pressed");
            loginSystem.loginUser = {
                email: '',
                password: ''
            },
                loginSystem.state = state.LOGIN;
        },
        logout: function () {
            console.log("logout button pressed");
            loginSystem.logout();
        },
        register: function () {
            console.log("register button pressed");
            loginSystem.newUser = {
                name: '',
                email: '',
                password: ''
            },
                loginSystem.state = state.REGISTER;
        }
    }
});

//Strictly this should be done as part of the initialisation of the Vue object, however figuring out which stage of initialisation is taking place after the firebase initialisation is a pain
// So we're just going to initialise it immediately before we need it and flag it to only be done once. Then it won't matter.
var initialised = false;
function initApp() {
    if (initialised) {
        return; // We only need to set this up once, otherwise we get multiple event-calls!
    }
    initialised = true;
    // Listening for auth state changes.
    firebase.auth().onAuthStateChanged(function (user) {
        if (user) {
            //User is signed in
            loginSystem.activeUser = user;
            if (loginSystem.state === state.REGISTER) {
                // If we're registering a new user, we need to add the Name we requested as part of this, so having created the user, we update the profile name to include it.
                console.log("updating user name");
                user.updateProfile({
                    displayName: loginSystem.newUser.name
                }).then(function () {
                    //When we get the response, we login succesfully
                    loginSystem.state = state.ACTIVE;
                    console.log("update successful");
                }).catch(function (error) {
                    // An error happened, but carry on, it's too late now.
                    loginSystem.state = state.ACTIVE;
                    console.log(error);
                });
            } else {
                //Regular login result.
                console.log("Logged In Successfully");
                loginSystem.state = state.ACTIVE;
            }
        }
    });
}