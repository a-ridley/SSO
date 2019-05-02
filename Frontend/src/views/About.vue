<template>
  <div class="about">
    <v-container fill-height id="wrapper">
      <v-layout align-center>
        <v-flex fill-height>
          <v-jumbotron height="100%">
            <h3 class="display-3">About KFC Single Sign On</h3>
            <v-divider class="my-3"></v-divider>
            <div class="title mb-3">Objective</div>
            <p class="subheading">To create a single sign-on platform that allows a user to have a uniform experience of all applications available to them through the means of a secure, customizable, and simple single sign-on portal.</p>
            <div class="title mb-3">Values</div>
            <p class="subheading">1. Simplify authorization and authentication</p>
            <p class="subheading">2. Increase awareness to the user of existing applications</p>
          </v-jumbotron>
        </v-flex>
      </v-layout>
    </v-container>
  </div>
</template>

<script>
export default {
  name: 'About'
}
</script>

<style scoped>

.wrapper {
  width: 70%;
  margin: 1px auto;
}
</style>
