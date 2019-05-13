<template>
  <v-layout id="appRegister">
    <div id="appRegister">
      <h1 class="display-1">Application Registration</h1>
      <v-divider class="my-3"></v-divider>
      <br />
      <v-form>
      <v-text-field
          name="title"
          id="title"
          v-model="title"
          type="title"
          label="Title" 
          v-if="!key"
          /><br />
      <v-text-field
          name="launchUrl"
          id="launchUrl"
          type="launchUrl"
          v-model="launchUrl"
          label="Launch Url" 
          v-if="!key"
          /><br />
      <v-text-field
          name="email"
          id="email"
          type="email"
          v-model="email"
          label="Email" 
          v-if="!key"
          /><br />
      <v-text-field
          name="healthCheckUrl"
          id="healthCheckUrl"
          type="healthCheckUrl"
          v-model="healthCheckUrl"
          label="Health Check Url" 
          v-if="!key"
          /><br />
      <v-text-field
          name="deleteUrl"
          id="deleteUrl"
          type="deleteUrl"
          v-model="deleteUrl"
          label="User Deletion Url" 
          v-if="!key"
          /><br />
      <v-text-field
          name="logoutUrl"
          id="logoutUrl"
          type="logoutUrl"
          v-model="logoutUrl"
          label="Logout Url" 
          v-if="!key"
          /><br />

      
      <v-alert
          :value="error"
          id="error"
          type="error"
          transition="scale-transition"
      >
          {{error}}
      </v-alert>

      <div id="responseMessage" v-if="message">
          <h3>{{ message }}</h3>
          <br />
      </div>
      <div id="applicationId" v-if="appId">
          <h3>Your Application ID</h3>
          <p>{{ appId }}</p>
      </div>
      <div id="apiKeyMessage" v-if="key">
          <h3>Your API Key:</h3>
          <p>{{ key }}</p>
      </div>
      <div id="secretKeyMessage" v-if="secretKey">
          <h3>Your Secret Key</h3>
          <p>{{ secretKey }}</p>
      </div>

      <br />

      <v-btn id="btnRegister" color="success" v-if="!key" v-on:click="register">Register</v-btn>

      </v-form>

      <Loading :dialog="loading" :text="loadingText" />
    </div>
  </v-layout>
</template>

<script>
import axios from 'axios'
import { apiURL } from '@/const.js'
import Loading from '@/components/Dialogs/Loading'

export default {
  components:{
    Loading,
  },
  data () {
    return {
      message: null,
      key: null,
      secretKey: null,
      appId: null,
      title: '',
      email: '',
      launchUrl: '',
      deleteUrl: '',
      healthCheckUrl: '',
      logoutUrl: '',
      error: '',
      loading: false,
      loadingText: "",
    }
  },
  methods: {
    register: function () {
      
      this.error = "";
      if (this.title.length == 0 || this.email.length == 0 || this.launchUrl.length == 0 || this.deleteUrl.length == 0 || this.healthCheckUrl.length == 0 || this.logoutUrl.length == 0) {
        this.error = "Fields Cannot Be Left Blank.";
      }

      if (this.error) return;

      const url = `${apiURL}/applications/create`
      this.loading = true;
      this.loadingText = "Registering App...";
      axios.post(url, {
        title: document.getElementById('title').value,
        launchUrl: document.getElementById('launchUrl').value,
        email: document.getElementById('email').value,
        deleteUrl: document.getElementById('deleteUrl').value,
        healthCheckUrl: document.getElementById('healthCheckUrl').value,
        logoutUrl: document.getElementById('logoutUrl').value,
        headers: {
          'Accept': 'application/json',
          'Content-Type': 'application/json'
        }
      })
        .then(response => {
            this.message = response.data.Message;
            this.key = response.data.Key; // Retrieve api key from response
            this.secretKey = response.data.SharedSecretKey // Retrieve shared api key from response
            this.appId = response.data.AppId // Retrieve application id from response
        })
        .catch(err => {
            this.error = err.response.data.Message;
        })
        .finally(() => {
          this.loading = false;
        })
    }
  }
}

</script>

<style lang="css">

#appRegister {
  width: 100%;
  padding: 15px;
  margin-top: 20px;
  max-width: 800px;
  margin: 1px auto;
  align: center;
}

#btnRegister {
  margin: 0px;
  margin-bottom: 15px;
  padding: 0px;
}

</style>
