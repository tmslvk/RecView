<template>
  <div class="block">
    <div class="columns is-mobile is-centered">
      <div class="column is-half">
        <div class="card">
          <form>
            <div class="title">
              <h1>Sign up</h1>
            </div>
            <div class="field">
              <label class="label">Username</label>
              <div class="control">
                <p class="control has-icons-left has-icons-right">
                  <input
                    v-bind:value="setupState.user.username"
                    @input="setupState.user.username = $event.target.value"
                    class="input"
                    type="text"
                    placeholder="e.g. Kiberslonyara"
                  />
                  <span class="icon is-small is-left">
                    <i class="fa-solid fa-user"></i>
                  </span>
                </p>
                <p
                  class="help is-danger"
                  v-if="v$.user.username.$error"
                >
                  {{v$.user.username.$errors[0].$message}}
                </p>
              </div>
            </div>
            <div class="field">
              <label class="label">Firstname</label>
              <div class="control">
                <p class="control has-icons-left has-icons-right">
                  <input
                    v-bind:value="setupState.user.firstname"
                    @input="setupState.user.firstname = $event.target.value"
                    class="input"
                    type="text"
                    placeholder="e.g. Alex"
                  />
                  <span class="icon is-small is-left">
                    <i class="fa-solid fa-f"></i>
                  </span>
                </p>
                <p
                  class="help is-danger"
                  v-if="v$.user.firstname.$error"
                >
                  {{v$.user.firstname.$errors[0].$message}}
                </p>
              </div>
            </div>
            <div class="field">
              <label class="label">Lastname</label>
              <div class="control">
                <p class="control has-icons-left has-icons-right">
                  <input
                    v-bind:value="setupState.user.lastname"
                    @input="setupState.user.lastname = $event.target.value"
                    class="input"
                    type="text"
                    placeholder="e.g. Romanov"
                  />
                  <span class="icon is-small is-left">
                    <i class="fa-solid fa-l"></i>
                  </span>
                </p>
                <p
                  class="help is-danger"
                  v-if="v$.user.lastname.$error"
                >
                  {{v$.user.lastname.$errors[0].$message}}
                </p>
              </div>
            </div>
            <div class="field">
              <label class="label">Email</label>
              <div class="control">
                <p class="control has-icons-left has-icons-right">
                  <input
                    v-bind:value="setupState.user.email"
                    @input="setupState.user.email = $event.target.value"
                    class="input"
                    type="email"
                    placeholder="e.g. romanovalex@gmail.com"
                  />
                  <span class="icon is-small is-left">
                    <i class="fas fa-envelope"></i>
                  </span>
                </p>
                <p
                  class="help is-danger"
                  v-if="v$.user.email.$error"
                >
                  {{v$.user.email.$errors[0].$message}}
                </p>
              </div>
            </div>
            <div class="field">
              <label class="label">Password</label>
              <div class="control">
                <p class="control has-icons-left has-icons-right">
                  <input
                    v-bind:value="setupState.user.password"
                    @input="setupState.user.password = $event.target.value"
                    class="input"
                    type="password"
                    placeholder="e.g. Jebrajmykh"
                  />
                  <span class="icon is-small is-left">
                    <i class="fa-solid fa-lock"></i>
                  </span>
                </p>
                <p
                  class="help is-danger"
                  v-if="v$.user.password.$error"
                >
                  {{v$.user.password.$errors[0].$message}}
                </p>
              </div>
            </div>
            <div class="field">
              <label class="label">Country</label>
              <div class="control">
                <p class="control has-icons-left has-icons-right">
                  <input
                    v-bind:value="setupState.user.country"
                    @input="setupState.user.country = $event.target.value"
                    class="input"
                    type="text"
                    placeholder="e.g. UK, US"
                  />
                  <span class="icon is-small is-left">
                    <i class="fa-regular fa-calendar"></i>
                  </span>
                </p>
                <p
                  class="help is-danger"
                  v-if="v$.user.country.$error"
                >
                  {{v$.user.country.$errors[0].$message}}
                </p>
              </div>
            </div>
          </form>
        </div>
        <div class="field is-grouped is-grouped-right">
          <button
            class="button is-link is-right"
            @click="fetchUsers"
          >Sign up</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import axios from "axios";
import { computed, reactive } from "vue";
import useVuelidate from "@vuelidate/core";
import { required, minLength, email } from "@vuelidate/validators";
import { passwordComplexityValidator } from "../scripts/validator.js";

export default {
  setup() {
    const setupState = reactive({
      user: {
        username: "",
        firstname: "",
        lastname: "",
        email: "",
        password: "",
        country: "",
        spotifyId: "",
      },
    });

    const rules = computed(() => {
      return {
        user: {
          username: { required, minLength: minLength(5) },
          firstname: { required },
          lastname: { required },
          email: { required, email },
          password: {
            required,
            minLength: minLength(8),
            passwordComplexity: passwordComplexityValidator,
          },
          country: { required },
        },
      };
    });

    const v$ = useVuelidate(rules, setupState);

    return {
      setupState,
      v$,
    };
  },

  methods: {
    async fetchUsers() {
      try {
        setTimeout(() => this.v$.$validate(), 1000);
        const data = { ...this.setupState.user };
        console.log(data);
        const response = await axios
          .post("https://localhost:7154/api/Auth/register", data, {
            headers: {
              "Content-Type": "application/json",
              accept: "text/plain",
            },
          })
          .catch((e) => console.log(e));
        console.log(response);
        localStorage.setItem("token", response.data);

        this.$store.dispatch("setUser");

        this.$router.push("/MainPage");
      } catch (e) {
        console.log(e);
        alert("Error");
      }
    },
  },
};
</script>

<style scoped>
.button {
  margin-top: 1rem;
}

.card {
  padding: 2rem;
}

.valid {
  color: lightgreen;
}

.inValid {
  color: lightcoral;
}
</style>
