<template>
  <v-flex xs12 sm6 offset-sm3>
    <h1 align="center" id="text">KFC Landing page...</h1>
  </v-flex>
</template>

<script>
import { store } from '@/services/request'
export default {
  name: 'Landing',

  created(){
      localStorage.removeItem('token')
      store.state.isLogin = false;

  }
}
</script>

<style scoped>
#text{
  align: center;
  padding: 10px;
  width: 100%;
}
</style>