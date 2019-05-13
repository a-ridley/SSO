<template>
  <v-layout id="notfound">
    <div id="notfound">
      <td class="justify-center layout px-0">
        <v-icon
        size="55"
        >
          remove_circle
        </v-icon>
      </td>
      <h1 id="notfoundtext" class="display-2">Oops... Page Not Found</h1>
    </div>
  </v-layout>
</template>

<script>
export default {
  name: 'NotFound'
}
</script>

<style scoped>
  #notfound{
  width: 100%;
  padding: 15px;
  margin-top: 20px;
  max-width: 800px;
  margin: 1px auto;
  align: center;
  text-align: center;
}

#notfoundtext {
  margin-top: 20px;
}
</style>
