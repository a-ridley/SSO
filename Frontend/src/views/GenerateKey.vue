<template>
    <div class="generate-wrapper">

        <h1 class="header">Generate a New API Key</h1>

        <br />
        <v-form>
        <v-text-field
            name="title"
            id="title"
            v-model="title"
            type="title"
            label="Application Title" 
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

        
        <v-alert
            :value="error"
            id="error"
            type="error"
            transition="scale-transition"
        >
            {{error}}
        </v-alert>

        <div v-if="message" id="responseMessage">
            <h3>{{ message }}</h3>
            <br />
        </div>
        <div v-if="key" id="keyMessage">
            <h3>Your New API Key:</h3>
            <p>{{ key }}</p>
        </div>

        <br />

        <v-btn id="btnGenerate" color="success" v-if="!key" v-on:click="generate">Generate</v-btn>

        </v-form>

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
              Loading
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
import { apiURL } from '@/const.js'

export default {
  data () {
    return {
      message: null,
      key: null,
      title: '',
      email: '',
      error: '',
      loading: false
    }
  },
  methods: {
    generate: function () {
      
      this.error = "";
      if (this.title.length == 0 || this.email.length == 0) {
        this.error = "Fields Cannot Be Left Blank.";
      }

      if (this.error) return;

      const url = `${apiURL}/applications/generatekey`
      this.loading = true;
      axios.post(url, {
        title: document.getElementById('title').value,
        email: document.getElementById('email').value,
        headers: {
          'Accept': 'application/json',
          'Content-Type': 'application/json'
        }
      })
        .then(response => {
            this.message = response.data.Message;
            this.key = response.data.Key; // Retrieve api key from response
        })
        .catch(err => {
            this.error = err.response.data.Message
        })
        .finally(() => {
          this.loading = false;
        })
    }
  }
}

</script>

<style lang="css">
.generate-wrapper {
    width: 70%;
    margin: 1px auto;
}

</style>
