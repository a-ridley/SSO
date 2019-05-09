<template>
  <v-layout id="settings" xs>
    <div id="settings">
      <h1 class="display-1">Account Settings</h1>
      <v-divider class="my-3"></v-divider>
      <v-flex>
        <v-card>
          <v-card-title primary-title>
            <div>
              <h3 class="headline mb-0">{{email}}</h3>
            </div>
          </v-card-title>
        </v-card>
      <br />
      <div>
        <h2 class="display">Password</h2>
        <v-btn id="settingButton" color="success" v-on:click="routeToUpdatePW">Update Password</v-btn>
      </div>
      <br/>
      <div>
        <h2 class="display">Deletion</h2>
        <v-btn id="settingButton" color="error" v-on:click="routeToDelete">Delete Account</v-btn>
      </div>
    </v-flex>
    </div>
  </v-layout>
</template>

<script>
import { store } from "@/services/request";

export default {
  name: 'AccountSettings',
  data(){
      return{
        email: store.state.email
      }
  },
  methods: {
    redirectToHome: function () {
      this.$router.push( "/home" )
    },
    routeToDelete: function () {
        this.$router.push("/deleteaccount")
    },
    routeToUpdatePW: function () {
        this.$router.push("/updatepassword")
    }
  }
}
</script>

<style>
#settings{
  width: 100%;
  padding: 15px;
  margin-top: 20px;
  max-width: 800px;
  margin: 1px auto;
  align: center;
}

#settingButton {
  margin: 0px
}
</style>

