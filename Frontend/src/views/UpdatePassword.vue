<template>
  <v-layout id="updatePassword">
    <div id="updatePassword">
      <v-alert
        :value="message" 
        dismissible 
        type="success"
      >{{message}}</v-alert>
      <v-alert
        :value="errorMessage"
        dismissible
        type="error"
        transition="scale-transition"
      >{{errorMessage}}</v-alert>
      <h1 class="display-1">Update Password</h1>
      <v-divider class="my-3"></v-divider>
      <div class="submitPasswords">
        <v-text-field
            name="oldPassword"
            id="oldPassword"
            v-model="oldPassword"
            type="password"
            label="Old Password"/>
        <br/>
        <v-text-field
            name="newPassword"
            id="newPassword"
            v-model="newPassword"
            type="password"
            label="New Password"/>
        <br />
        <v-text-field
              name="confirmNewPassword"
              id="confirmNewPassword"
              v-model="confirmNewPassword"
              type="password"
              label="Confirm New Password"/>
        <br />
        <v-btn id="submit" color="success" v-on:click="submitPasswords">Update Password</v-btn>
      </div>
    </div>
  </v-layout>
</template>

<script>
import axios from 'axios';
import { apiURL } from '@/const.js';

export default {
  name: 'UpdatePassword',
  data () {
    return {
      message: null,
      errorMessage: null,
      oldPassword: null,
      newPassword: null,
      confirmNewPassword: null
    }
  },
  methods: {
    submitPasswords: function () {
      if(this.newPassword == null || this.confirmNewPassword == null || this.oldPassword == null){
        this.errorMessage = 'Password fields cannot be empty'
      }
      else if (this.newPassword.length < 12 || this.oldPassword.length < 12 || this.confirmNewPassword.length < 12) {
        this.errorMessage = 'Password does not meet minimum length of 12'
      } else if (this.newPassword.length > 2000 || this.oldPassword.length > 2000 || this.confirmNewPassword.length > 2000) {
        this.errorMessage = 'Password exceeds maximum length of 2000'
      } else if (this.confirmNewPassword !== this.newPassword) {
        this.errorMessage = 'Passwords do not match'
      } else if (this.oldPassword === this.newPassword){
        this.errorMessage = 'Cannot use the same password to update'
      } else {
        axios({
          method: 'POST',
          url: `${apiURL}/users/updatepassword`,
          data: {
            sessionToken: localStorage.token,
            oldPassword: this.$data.oldPassword,
            newPassword: this.$data.confirmNewPassword
          },
          headers: {
            'Access-Control-Allow-Origin': '*',
            'Access-Control-Allow-Credentials': true
          }
        })
          .then(response => {this.message = response.data})
          .catch(e => { this.errorMessage = e.response.data })
      }
    }
  }
}
</script>

<style>

#update{
  width: 70%;
  margin: 1px auto;
}

#updatePassword {
  width: 100%;
  padding: 15px;
  margin-top: 20px;
  max-width: 800px;
  margin: 1px auto;
  align: center;
}

</style>
