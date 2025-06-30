<template>
    <div>
        <div class="text-2xl font-bold mb-4">
            Room Management - Login
        </div>
        <FormKit 
            type="form"
            :actions="false"
            @submit="onSubmit"
        >
            <FormKit 
                label="Email"
                v-model="userLogin.email"
            />
            <FormKit
                label="Password"
                type="password"
                v-model="userLogin.password"
            />
            <FormKit 
                label="Login"
                type="submit"
            />
        </FormKit>
    </div>
</template>

<script setup lang="ts">
    const userLogin =ref({
        email: undefined,
        password: undefined
    })

    const onSubmit=()=>{
        $fetch('/api/v1/User/Login',{
            server:false,
            method: 'POST',
            body: userLogin.value,
            onResponse({response}){
                if(!response.ok){
                    alert('Cannot login')
                }
                else{
                    localStorage.setItem('token', response._data)
                    navigateTo('/')
                }
                
            }
        })
    }
</script>