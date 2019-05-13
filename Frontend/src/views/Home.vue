<template>
  <v-flex xs12 sm6 offset-sm3>
    <img src="@/assets/SPG_Logo_Banner.png" id="banner"/>
    <h1 align="center" id="text">Welcome to the SPG Single Sign-On portal!</h1>
  </v-flex>
</template>

<script>
export default {
  name: 'Home',
  created() {
    const token = localStorage.getItem("token");
    if(!token){
      this.$router.push('/login');
    }
    else{
      this.$router.push('/dashboard');
    }
  },
}
</script>

<style scoped>
#banner{
  width: 100%;
  height: 100%;
  align: center;
}

#text{
  align: center;
  padding: 10px;
  width: 100%;
}
</style>