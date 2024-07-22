<template>
  <div class="app">
    <div v-if="user !== null ">
      <navbar />
    </div>
    <div class="block">
    </div>
    <router-view />
  </div>
</template>

<script>
import { mapState } from "vuex";
import LoginForm from "./components/LoginForm.vue";

export default {
  computed: mapState(["user"]),
  components: {
    LoginForm,
  },
  mounted() {
    setTimeout(() => {
      this.$store.dispatch("setUser");
    }, 2000);
  },
};
</script>


