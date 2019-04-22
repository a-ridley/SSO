<template>
  <div id="settings">
    <div id="AccountSettings">
    </div>
     <h1>Account Settings</h1>
    <br />
    <br />
    <div class="">
    <br/>
    <v-btn color="success" v-on:click="routeToUpdatePW">Update Password</v-btn>
    </div>
    <br/>
    <div class="">
        <br/>
        <v-btn color="error" v-on:click="routeToDelete">Delete Account</v-btn>
    </div>
  </div>
</template>

<script>
export default {
  name: 'AccountSettings',
  data(){
      return{
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
  width: 70%;
  margin: 1px auto;
}
</style>

