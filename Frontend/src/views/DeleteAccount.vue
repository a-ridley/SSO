<template>
  <div id="delete">
    <div id="DeleteAccount">
      <v-alert
      :value="message"
      dismissible
      type="success"
    >
      {{message}}
    </v-alert>

    <v-alert
      :value="error"
      dismissible
      type="error"
      transition="scale-transition"
    >
    {{error}}
    </v-alert>

    </div>
     <h1>Account Deletion</h1>
    <br />
    <br />
    <div class="">
        <br/>
        <v-btn color="error" @click.stop="dialog = true">Delete My Account</v-btn>
    </div>
    <v-dialog
      v-model="dialog"
      width="300"
    >
      <v-card>
        <v-card-title class="headline">Confirm Deletion</v-card-title>

        <v-card-actions>
          <v-spacer></v-spacer>

          <v-btn
            color="success"
            @click="dialog = false"
          >
          Cancel
          </v-btn>

          <v-btn
            color="error"
            @click="dialog = false"
            v-on:click="runDelete"
          >
          Delete
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
    <v-dialog
      v-model="loading"
      hide-overlay
      persistent
      width="300"
    >
      <v-card
        color="primary"
        dark
      >
        <v-card-text>
          Deleting
          <v-progress-linear
            indeterminate
            color="white"
            class="mb-0"
          ></v-progress-linear>
        </v-card-text>
      </v-card>
    </v-dialog>
  </div>
</template>

<script>
import axios from 'axios'
import { apiURL } from '@/const.js';
import { store } from '@/services/request'
export default {
  name: 'DeleteAccount',
  data(){
      return{
          token: "",
          error: "",
          message: "",
          loading: false,
          dialog: false
      }
  },
  methods: {
    redirectToHome: function () {
      this.$router.push( "/home" )
    },
    runDelete: function () {
        this.loading = true;
          axios({
          method: 'POST',
          url: `${apiURL}/users/deleteuser`,
          data: {
            token: localStorage.token,
          },
          headers: {
            'Access-Control-Allow-Origin': '*',
            'Access-Control-Allow-Credentials': true
          }
        })
        .then(response => {
          this.message = response.data;
          localStorage.removeItem('token'),
          store.state.isLogin = false ,
          this.redirectToHome()
          
        })
        .catch(() => { this.error = "Failed to delete user, try again" })
        .finally(() => { this.loading = false; })
    }
  }
}
</script>

<style>
#delete{
  width: 70%;
  margin: 1px auto;
}
</style>
