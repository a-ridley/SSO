<template>
  <v-toolbar id="toolbar" app>
    <router-link
      to="/dashboard"
      tag="v-btn"
      color="white"
      id="logoNav">
      <v-btn id="iconBtn" flat>
        <v-img :src="image" id="logo"></v-img>
      </v-btn>
    </router-link>
    <v-toolbar-title id="toolbarTitle" class="headline text-uppercase">
      <span>SPG SSO</span>
    </v-toolbar-title>
    <v-spacer></v-spacer>
    <v-toolbar-items class="hidden-sm-and-down" >
      <v-btn v-if="!isLoggedIn.isLogin" to="/home" flat>Home</v-btn>
      <v-btn v-else to="/dashboard" flat>Home</v-btn>
      <v-btn to="/register" flat v-if="!isLoggedIn.isLogin">Register</v-btn>
      <v-btn to="/about" flat>About</v-btn>
      <v-menu offset-y id="appDropDown">
        <template slot="activator">
          <v-btn flat>
            <span>Application</span>
            <v-icon>expand_more</v-icon>
          </v-btn>
        </template>
        <v-list dense>
          <v-list-tile v-for="link in appLinks"
                        :key="link.text"
                        router :to="link.route" >
            <v-list-tile-title>{{link.text}}</v-list-tile-title>
          </v-list-tile>
        </v-list>
      </v-menu>
    </v-toolbar-items>
    <v-menu class="hidden-md-and-up">
      <v-toolbar-side-icon slot="activator"></v-toolbar-side-icon>
        <v-list dense>
          <v-list-tile v-for="link in expandMenu"
                        :key="link.text"
                        router :to="link.route"
                        v-if="link.display" >
            <v-list-tile-title >{{link.text}}</v-list-tile-title>
          </v-list-tile>
        </v-list>
    </v-menu>
    <div>
      <v-btn to="login" flat v-if="!isLoggedIn.isLogin">Login</v-btn>
      <v-menu offset-y
              content-class="dropdown-menu"
              transition="slide-y-transition" v-if="isLoggedIn.isLogin">
        <v-btn id="avatar" slot="activator" fab dark color="teal">
          <v-avatar dark>
            <span class="white--text headline">{{isLoggedIn.email[0]}}</span>
          </v-avatar>
        </v-btn>
        <v-list dense>
          <v-list-tile 
            v-for="item in this.UserMenuItems"
            :key="item.title"
            route :to="item.route">
            <v-list-tile-title>{{item.title}}</v-list-tile-title>
          </v-list-tile>
        </v-list>
      </v-menu>
    </div>
  </v-toolbar>
</template>

<script>
  import { store } from '@/services/request'
  export default {
    name: 'NavBar',
    data() {
      return {
        image: require("@/assets/SPG_SSO_Logo_T.png"),
        appLinks: [
            { text: 'Register', route: '/add', display: true },
            { text: 'Generate Key', route: '/key', display: true },
            { text: 'Delete', route: '/delete', display: true },
        ],
        expandMenu: [
          { text: 'Home', route: '/', display: true},
          { text: 'About', route: '/about', display: true},
          { text: 'Register', route: '/register', display: true},
          { text: 'App Register', route: '/add', display: true },
          { text: 'App Generate Key', route: '/key', display: true },
          { text: 'App Delete', route: '/delete', display: true },
        ],
        links: [],
        UserMenuItems: [
          { title: 'Account Settings', route: '/accountsettings'},
          { title: 'Logout', route: '/logout' },      
        ],
        isLoggedIn: store.state
      }
    },
    mounted() {
      if(this.$route.path === '/landing'){
        this.$router.push( "/home" )
      }
      else{
        store.isUserLogin()
      }
      if (store.state.isLogin === true) {
          store.getEmail()
      }
    }
  }
</script>

<style>
#logoNav {
  height: 100%;
  padding: 0px;
  margin-left: -24px;
}

#iconBtn {
  max-width: 41px;
  height: 100%;
  padding: 0px;
  margin: 0px;
}

#toolbarTitle {
  margin-left: 0px;
}

#toolbar {
  padding-left: 0px;
}

#logo {
  max-width: 57px;
  height: 57px;
}

#avatar {
  height: 50px;
  width: 50px
}
</style>