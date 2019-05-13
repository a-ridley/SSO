<template>
  <v-layout id="sendLink">
    <div id="sendLink">
      <h1 class="display-1">Reset Password</h1>
      <v-divider class="my-3"/>
      <v-form>
      <v-text-field
          name="email"
          id="email"
          v-model="email"
          type="email"
          label="Email" 
          /><br 
      />
      <v-alert
        :value="errorMessage"
        dismissible=""
        type="error"
        transition="scale-transition"
      >
      {{errorMessage}}
      </v-alert>
      </v-form>

      <v-alert
        :value="message"
        dismissible
        type="success"
        transition="scale-transition"
      >
      {{message}}
      </v-alert>

      <v-btn id="sendEmail" color="success" v-on:click="submitEmail">Send Email</v-btn>
      <Loading :dialog="loading" :text="loadingText" />
    </div>
  </v-layout>
</template>

<script>
import axios from 'axios';
import { apiURL } from '@/const.js';
import Loading from '@/components/Dialogs/Loading';

export default {
  name: 'SendResetLink',
  components: {
    Loading
  },
  data () {
    return {
      errorMessage: "",
      message: "",
      email: "",
      loading: false,
      loadingText: "",
    }
  },
  methods: {
    submitEmail: function () {
      if (!this.email) {
        this.errorMessage = 'Email field cannot be empty'
      } else if (!this.validEmail(this.email)) {
        this.errorMessage = 'Valid email required'
      } else {
        this.loading = true;
        this.loadingText = "Sending Email...";
        axios({
          method: 'POST',
          url: `${apiURL}/reset/send`,
          data: {email: this.$data.email, url: 'https://kfc-sso.com/#/resetpassword/'},
          headers: {
            'Access-Control-Allow-Origin': '*',
            'Access-Control-Allow-Credentials': true
          }
        })
          .then(response => {this.message = response.data}, this.errorMessage = '', 
          setTimeout(() => this.redirectToLogin(), 3000))
          .catch(e => { this.errorMessage = e.responsed.data })
          .finally(() => {
            this.loading = false;
          })
      }
    },
    validEmail: function (email) {
      var re = /^(([^<>()\\\\.,;:\s@"]+(\.[^<>()\\\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/
      return re.test(email)
    }

  }
}
</script>

<style>
#sendLink{
  width: 100%;
  padding: 15px;
  margin-top: 20px;
  max-width: 800px;
  margin: 1px auto;
  align: center;
}

#sendEmail {
  margin: 0px;
  margin-bottom: 15px;
}
</style>
